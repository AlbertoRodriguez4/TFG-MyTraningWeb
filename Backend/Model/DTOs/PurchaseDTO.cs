namespace AA2_CS.Model.DTOs
{
    // DTO para representar una compra realizada por un usuario. Incluye información sobre el usuario, el ítem comprado, el precio, la fecha de compra y la URL de la imagen del ítem.
    public class PurchaseDTO
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int ItemBonus { get; set; }
        public int ItemPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string? ImageUrl { get; set; } 
    }
}
