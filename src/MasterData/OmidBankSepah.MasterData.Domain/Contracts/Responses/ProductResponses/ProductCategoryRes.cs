namespace OmidBankSepah.MasterData.Domain.Contracts.Responses.ProductResponses;

public sealed record ProductCategoryRes(
    Guid Id,
    bool IsActive,
    string Name,
    string Code,
    int? ParentProductCategoryId,
    string Description,
    IReadOnlyList<ProductRes> Products);
