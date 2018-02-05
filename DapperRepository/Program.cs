using System;

namespace DapperRepository
{
    class Program
    {
        static void Main(string[] args)
        {
            var productRepository = new Repository<Product>();

            for (int i = 0; i < 500; i++)
            {
                var product = new Product
                {
                    Name = "Ipad Pro 13 inch 64 GB",
                    Price = 2480m,
                    CreatedDate = DateTime.Now
                };

                //insert
                productRepository.Insert(product);
            }

            ////find by identifier
            //product = productRepository.Find(product.Id);
            //product.Price = 2999m;

            ////update
            //productRepository.Update(product);

            ////delete
            //productRepository.Delete(product);

            //find all with expression
            var products = productRepository.FindAll(x => x.Id != 0);
            foreach (var item in products)
            {
                Console.WriteLine("Found: {0} - {1}", item.Name, item.Price);
            }

            Console.ReadKey();
        }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OldPrice { get; set; }
        public decimal Price { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public bool? IsShow { get; set; }
        public string ContentText { get; set; }
        public long DisplayOrder { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}