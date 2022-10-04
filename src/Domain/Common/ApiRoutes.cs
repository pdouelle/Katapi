namespace Domain.Common;

public static class ApiRoutes
{
    public const string Root = "api";
    public const string ContentType = "application/json";

    public static class Products
    {
        public const string Id = "productId";
        public const string GetList = "products";
        public const string GetById = $"products/{{{Id}}}";
        public const string Post = "products";
        public const string Put = $"products/{{{Id}}}";
        public const string Patch = $"products/{{{Id}}}";
        public const string Delete = $"products/{{{Id}}}";
    }
}