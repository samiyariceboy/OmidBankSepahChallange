namespace OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductResponses;

public sealed record ProductRes(
    Guid Id,
    Guid ProductCategoryId,
    string ProductName,
    string BrandName,
    string ModelName,
    int ProductYear,
    string ProductDescription);
