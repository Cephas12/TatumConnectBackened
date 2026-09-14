namespace TatumConnectBackened.DTOs
{
    public class ProductRequiredFieldsDto
    {
        public List<ProductFieldDefinitionDto>? Fields { get; set; }
    }

    public class ProductFieldDefinitionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool Required { get; set; }
        public List<string>? AllowedValues { get; set; }
        public ProductFieldValidationDto? Validation { get; set; }
    }

    public class ProductFieldValidationDto
    {
        public string? Pattern { get; set; }
    }
}
