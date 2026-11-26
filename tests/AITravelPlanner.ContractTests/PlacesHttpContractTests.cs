using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AITravelPlanner.Domain.Places;

namespace AITravelPlanner.ContractTests;

public sealed class PlacesHttpContractTests
{
    private static readonly string[] PageFields = ["items", "page", "pageSize", "totalItems", "totalPages"];
    private static readonly string[] PlaceFields = ["category", "district", "id", "name"];
    private static readonly string[] Categories = ["cafe", "food", "attraction"];
    private static readonly string[] Districts = ["q1", "q3"];
    private const string InvalidQuery = "urn:travel-planner:problem:invalid-query";
    private const string InternalError = "urn:travel-planner:problem:internal-error";

    [Theory]
    [InlineData("", "1,2,3,4,5,6", 1, 10, 6, 1)]
    [InlineData("?page=1&pageSize=10", "1,2,3,4,5,6", 1, 10, 6, 1)]
    [InlineData("?category=cafe", "1,4,6", 1, 10, 3, 1)]
    [InlineData("?district=q3", "4,5", 1, 10, 2, 1)]
    [InlineData("?category=food", "2,5", 1, 10, 2, 1)]
    [InlineData("?category=attraction", "3", 1, 10, 1, 1)]
    [InlineData("?district=q1", "1,2,3,6", 1, 10, 4, 1)]
    [InlineData("?category=cafe&district=q1&page=2&pageSize=1", "6", 2, 1, 2, 2)]
    [InlineData("?page=1&pageSize=2", "1,2", 1, 2, 6, 3)]
    [InlineData("?page=2&pageSize=2", "3,4", 2, 2, 6, 3)]
    [InlineData("?page=3&pageSize=2", "5,6", 3, 2, 6, 3)]
    [InlineData("?page=4&pageSize=2", "", 4, 2, 6, 3)]
    [InlineData("?page=1&pageSize=1", "1", 1, 1, 6, 6)]
    [InlineData("?page=100000&pageSize=50", "", 100000, 50, 6, 1)]
    [InlineData("?category=attraction&district=q3", "", 1, 10, 0, 0)]
    [InlineData("?category=attraction&district=q3&page=5", "", 5, 10, 0, 0)]
    [InlineData("?page=01&pageSize=01", "1", 1, 1, 6, 6)]
    [InlineData("?page=000000000000000000000000000000000000000001", "1,2,3,4,5,6", 1, 10, 6, 1)]
    [InlineData("?%70age=%31&category=%63afe", "1,4,6", 1, 10, 3, 1)]
    [InlineData("?&&", "1,2,3,4,5,6", 1, 10, 6, 1)]
    [InlineData("?category=food&district=q3", "5", 1, 10, 1, 1)]
    public async Task Success_matches_public_schema_filter_order_and_metadata(
        string query, string ids, int page, int size, int total, int pages)
    {
        var catalog = new FakeCatalog();
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places" + query);
        await AssertPage(response, catalog.Places, ids, page, size, total, pages);
        Assert.Equal(1, catalog.Reads);
    }

    public static TheoryData<string, string> InvalidQueries()
    {
        var data = new TheoryData<string, string>();
        data.Add("?sort=id", "sort");
        data.Add("?Page=1", "Page");
        data.Add("?=x", "");
        data.Add("?%20page=1", " page");
        data.Add("?unknown=one&unknown=two", "unknown");
        data.Add("?Page=1&page=2", "page");
        data.Add("?page=1&Page=2", "Page");
        data.Add("?%70age=1&page=1", "page");
        foreach (var key in new[] { "page", "pageSize", "category", "district" })
        {
            var valid = key switch { "category" => "cafe", "district" => "q1", _ => "1" };
            data.Add($"?{key}=", key);
            data.Add($"?{key}", key);
            data.Add($"?{key}={valid}&{key}={valid}", key);
            data.Add($"?{key}={valid}&{key}=different", key);
        }
        foreach (var key in new[] { "page", "pageSize" })
        {
            foreach (var value in new[] { "abc", "1.0", "1e2", "-1", "\uFF11", " 1", "+1", "1 ", "0", new string('9', 100) })
                data.Add($"?{key}={Uri.EscapeDataString(value)}", key);
        }
        data.Add("?page=100001", "page");
        data.Add("?pageSize=51", "pageSize");
        data.Add("?page=+1", "page");
        foreach (var value in new[] { "Cafe", "museum", "cafe ", "%63afe", "cafe;district=q1" })
            data.Add($"?category={Uri.EscapeDataString(value)}", "category");
        foreach (var value in new[] { "Q1", "q2", "q1 " })
            data.Add($"?district={Uri.EscapeDataString(value)}", "district");
        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public async Task Invalid_query_returns_problem_before_catalog_access(string query, string key)
    {
        var catalog = new FakeCatalog { ThrowOnRead = true };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places" + query);
        var body = await AssertProblem(response, 400, InvalidQuery);
        AssertErrors(body, key);
        Assert.Equal(0, catalog.Reads);
    }

    [Theory]
    [InlineData("?page=0&pageSize=&category=Cafe&sort=id", "page,pageSize,category,sort")]
    [InlineData("?page=&page=x&district=Q1&other=1", "page,district,other")]
    public async Task Invalid_query_reports_every_invalid_group(string query, string keys)
    {
        var catalog = new FakeCatalog { ThrowOnRead = true };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places" + query);
        AssertErrors(await AssertProblem(response, 400, InvalidQuery), keys.Split(','));
        Assert.Equal(0, catalog.Reads);
    }

    [Theory]
    [InlineData("anonymous")]
    [InlineData("authorization")]
    [InlineData("accept")]
    [InlineData("body")]
    [InlineData("idempotency")]
    public async Task Public_read_ignores_irrelevant_headers_and_body(string variant)
    {
        var catalog = new FakeCatalog();
        var before = catalog.Places.ToArray();
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/places");
        if (variant == "authorization")
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-not-a-real-token");
            request.Headers.Add("Cookie", "session=irrelevant");
        }
        if (variant == "accept") request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        if (variant == "body") request.Content = new StringContent("not-json", Encoding.UTF8, "application/json");
        if (variant == "idempotency") request.Headers.Add("Idempotency-Key", "ignored");
        using var response = await client.SendAsync(request);
        await AssertPage(response, catalog.Places, "1,2,3,4,5,6", 1, 10, 6, 1);
        Assert.False(response.Headers.Contains("WWW-Authenticate"));
        Assert.Equal(before, catalog.Places);
    }

    [Fact]
    public async Task Empty_catalog_returns_zero_totals()
    {
        var catalog = new FakeCatalog { Places = [] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places?page=5");
        await AssertPage(response, catalog.Places, "", 5, 10, 0, 0);
    }

    [Fact]
    public async Task Consecutive_pages_do_not_overlap_and_retries_do_not_mutate_snapshot()
    {
        var catalog = new FakeCatalog();
        var before = catalog.Places.ToArray();
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var first = await client.GetAsync("/places?pageSize=2");
        using var second = await client.GetAsync("/places?pageSize=2&page=2");
        using var retry = await client.GetAsync("/places?pageSize=2");
        var a = await AssertPage(first, catalog.Places, "1,2", 1, 2, 6, 3);
        var b = await AssertPage(second, catalog.Places, "3,4", 2, 2, 6, 3);
        var c = await AssertPage(retry, catalog.Places, "1,2", 1, 2, 6, 3);
        Assert.Empty(a.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("id").GetInt32())
            .Intersect(b.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("id").GetInt32())));
        Assert.True(JsonElement.DeepEquals(a, c));
        Assert.Equal(before, catalog.Places);
        Assert.Equal(3, catalog.Reads);
    }

    [Fact]
    public async Task Numeric_sort_and_changed_catalog_have_coherent_per_response_totals()
    {
        var catalog = new FakeCatalog { Places = [ReferenceFixture.Valid(10), ReferenceFixture.Valid(2), ReferenceFixture.Valid(1)] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var first = await client.GetAsync("/places");
        await AssertPage(first, catalog.Places, "1,2,10", 1, 10, 3, 1);
        catalog.Places = [ReferenceFixture.Valid(4)];
        using var second = await client.GetAsync("/places?page=2&pageSize=1");
        await AssertPage(second, catalog.Places, "", 2, 1, 1, 1);
        Assert.Equal(2, catalog.Reads);
    }

    public static TheoryData<string> Exclusions() => new()
    {
        "unverified", "unpublished", "private", "draft", "outside-city", "ineligible", "missing-hours", "empty-hours", "missing-menu", "empty-menu", "invalid-but-excluded"
    };

    [Theory]
    [MemberData(nameof(Exclusions))]
    public async Task Each_inclusion_condition_is_required_before_filtering_and_counting(string scenario)
    {
        var valid = ReferenceFixture.Valid(99);
        var excluded = scenario switch
        {
            "unverified" => valid with { Verified = false },
            "unpublished" => valid with { Published = false },
            "private" => valid with { Private = true },
            "draft" => valid with { Draft = true },
            "outside-city" => valid with { City = "OTHER" },
            "ineligible" => valid with { Eligible = false },
            "missing-hours" => valid with { OpeningHours = null },
            "empty-hours" => valid with { OpeningHours = [] },
            "missing-menu" => valid with { Menu = null },
            "empty-menu" => valid with { Menu = [] },
            "invalid-but-excluded" => valid with { Verified = false, Name = null },
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        var catalog = new FakeCatalog { Places = [ReferenceFixture.Valid(1), excluded] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var all = await client.GetAsync("/places");
        await AssertPage(all, catalog.Places, "1", 1, 10, 1, 1);
        using var filtered = await client.GetAsync("/places?category=cafe&district=q1");
        await AssertPage(filtered, catalog.Places, "1", 1, 10, 1, 1);
    }

    public static TheoryData<string> InvalidRecords() => new()
    {
        "zero-id", "negative-id", "null-name", "empty-name", "leading-space", "trailing-space", "control-character", "long-name", "null-category", "case-category", "unknown-category", "null-district", "unknown-district"
    };

    [Theory]
    [MemberData(nameof(InvalidRecords))]
    public async Task Invalid_included_record_is_safe_internal_error_even_if_filter_would_hide_it(string scenario)
    {
        var valid = ReferenceFixture.Valid();
        var invalid = scenario switch
        {
            "zero-id" => valid with { Id = 0 },
            "negative-id" => valid with { Id = -1 },
            "null-name" => valid with { Name = null },
            "empty-name" => valid with { Name = "" },
            "leading-space" => valid with { Name = " leading" },
            "trailing-space" => valid with { Name = "trailing " },
            "control-character" => valid with { Name = "control\ncharacter" },
            "long-name" => valid with { Name = new string('x', 201) },
            "null-category" => valid with { Category = null },
            "case-category" => valid with { Category = "Cafe" },
            "unknown-category" => valid with { Category = "museum" },
            "null-district" => valid with { District = null },
            "unknown-district" => valid with { District = "q2" },
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        var catalog = new FakeCatalog { Places = [invalid] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places?category=food");
        await AssertProblem(response, 500, InternalError);
        Assert.Equal(1, catalog.Reads);
    }

    [Fact]
    public async Task Duplicate_included_ids_are_safe_internal_error()
    {
        var catalog = new FakeCatalog { Places = [ReferenceFixture.Valid(), ReferenceFixture.Valid()] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places");
        await AssertProblem(response, 500, InternalError);
    }

    [Fact]
    public async Task Catalog_exception_is_safe_internal_error_without_debug_route()
    {
        var catalog = new FakeCatalog { ThrowOnRead = true };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places");
        await AssertProblem(response, 500, InternalError);
        Assert.Equal(1, catalog.Reads);
    }

    [Theory]
    [InlineData("minimum")]
    [InlineData("maximum")]
    [InlineData("unicode")]
    [InlineData("nullable-eligibility")]
    public async Task Valid_public_field_boundaries_are_accepted(string scenario)
    {
        var place = ReferenceFixture.Valid() with
        {
            Id = int.MaxValue,
            Name = scenario switch { "minimum" => "x", "unicode" => string.Concat(Enumerable.Repeat("\U0001F600", 200)), _ => new string('x', 200) },
            Eligible = scenario == "nullable-eligibility" ? null : true
        };
        var catalog = new FakeCatalog { Places = [place] };
        using var host = new PlacesTestHost(catalog);
        using var client = host.CreateHttpsClient();
        using var response = await client.GetAsync("/places");
        await AssertPage(response, catalog.Places, int.MaxValue.ToString(), 1, 10, 1, 1);
    }

    private static void AssertHeaders(HttpResponseMessage response, string mediaType)
    {
        Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Headers.CacheControl?.NoStore);
        Assert.False(response.Headers.Contains("Idempotent-Replayed"));
        Assert.False(response.Headers.Contains("ETag"));
    }

    private static async Task<JsonElement> AssertPage(HttpResponseMessage response, IReadOnlyList<Place> fixture,
        string ids, int page, int size, int total, int pages)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AssertHeaders(response, "application/json");
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.Equal(PageFields.Order(),
            root.EnumerateObject().Select(p => p.Name).Order());
        Assert.Equal(page, Integer(root, "page"));
        Assert.Equal(size, Integer(root, "pageSize"));
        Assert.Equal(total, Integer(root, "totalItems"));
        Assert.Equal(pages, Integer(root, "totalPages"));
        var items = root.GetProperty("items");
        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        var expected = ids.Length == 0 ? [] : ids.Split(',').Select(int.Parse).ToArray();
        Assert.Equal(expected, items.EnumerateArray().Select(i => Integer(i, "id")).ToArray());
        Assert.True(items.GetArrayLength() <= size);
        foreach (var item in items.EnumerateArray())
        {
            var id = Integer(item, "id");
            Assert.True(id > 0);
            var source = Assert.Single(fixture, p => p.Id == id);
            Assert.Equal(PlaceFields, item.EnumerateObject().Select(p => p.Name).Order());
            Assert.Equal(source.Name, Text(item, "name"));
            Assert.Equal(source.Category, Text(item, "category"));
            Assert.Equal(source.District, Text(item, "district"));
            Assert.Contains(Text(item, "category"), Categories);
            Assert.Contains(Text(item, "district"), Districts);
        }
        return root.Clone();
    }

    private static int Integer(JsonElement value, string key)
    {
        Assert.Equal(JsonValueKind.Number, value.GetProperty(key).ValueKind);
        Assert.True(value.GetProperty(key).TryGetInt32(out var number));
        return number;
    }

    private static string Text(JsonElement value, string key)
    {
        Assert.Equal(JsonValueKind.String, value.GetProperty(key).ValueKind);
        return Assert.IsType<string>(value.GetProperty(key).GetString());
    }

    private static async Task<JsonElement> AssertProblem(HttpResponseMessage response, int status, string type)
    {
        Assert.Equal(status, (int)response.StatusCode);
        AssertHeaders(response, "application/problem+json");
        var text = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("SECRET_SENTINEL", text);
        Assert.DoesNotContain("InvalidOperationException", text);
        Assert.DoesNotContain("connection failure", text);
        Assert.DoesNotContain("stackTrace", text);
        using var document = JsonDocument.Parse(text);
        var root = document.RootElement;
        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.Equal(type, Text(root, "type"));
        Assert.Equal(status, Integer(root, "status"));
        Assert.Equal(status == 400 ? "Invalid query" : "Internal error", Text(root, "title"));
        Assert.Equal("/places", Text(root, "instance"));
        Assert.False(string.IsNullOrWhiteSpace(Text(root, "detail")));
        if (status == 500)
        {
            Assert.Equal("An unexpected error occurred. Please try again later.", Text(root, "detail"));
            Assert.False(root.TryGetProperty("errors", out _));
        }
        return root.Clone();
    }

    private static void AssertErrors(JsonElement problem, params string[] keys)
    {
        var errors = problem.GetProperty("errors");
        Assert.Equal(JsonValueKind.Object, errors.ValueKind);
        Assert.Equal(keys.Order(), errors.EnumerateObject().Select(p => p.Name).Order());
        foreach (var property in errors.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.Array, property.Value.ValueKind);
            var message = Assert.Single(property.Value.EnumerateArray());
            Assert.Equal(JsonValueKind.String, message.ValueKind);
            Assert.False(string.IsNullOrWhiteSpace(message.GetString()));
        }
    }
}
