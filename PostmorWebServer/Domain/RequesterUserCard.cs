namespace PostmorWebServer.Domain
{
    public class RequesterUserCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string Picture { get; set; }
    }
}
