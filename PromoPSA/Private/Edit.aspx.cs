using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.PromoPSA.BusinessEntities;
using KMI.PromoPSA.BusinessEntities.Classification;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.Web.UI;

public partial class Private_Edit : PrivateWebPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        DisconnectUserWebControl1.WebSession = _webSession;
        LoginInformationWebControl1.WebSession = _webSession;
        PromotionInformationWebControl1.WebSession = _webSession;
    }

       [WebMethod]
    public static void SaveCodification(Advert advert)
       {
           IResults results = new Results();
            results.UpdateCodification(advert);
       }
    [WebMethod]
    public static Codification GetCodification(long idForm)
    {      
        IResults results = new Results();
        return results.GetCodification(idForm);
    }

    [WebMethod]
    public static Advert GetAdvert()
    {
        Advert advert = new Advert
            {
                IdSegment = 3,
                IdProduct = 24,
                IdBrand = 2,
                PromotionContent = " voilà le conteu de la promo",
                ConditionText = "ce sont les conditions de la promo",
                Script = " voici le script",
                DateBeginNum = 20130910,
                DateEndNum = 20130913,
                DateMediaNum = 20130909,
                LoadDate = 201309,
                ExcluWeb = 1
            };

        return advert; 
    }

    [WebMethod]
    public static List<Segment> GetSegments()
    {
        List<Segment> segments = new List<Segment>();
        string result = null;
        IResults results = new Results();
        Segment segment = new Segment
            {
                Id = 2,
                Label = "Usure"
            };
        segments.Add(segment);
        segment = new Segment
            {
                Id = 3,
                Label = "Pneumatique"
            };
        segments.Add(segment);
        segment = new Segment
            {
                Id = 4,
                Label = "Usure"
            };
        segments.Add(segment);
        return segments;
    }

    [WebMethod]
    public static List<Product> GetProducts()
    {
        List<Product> products = new List<Product>();
        string result = null;
        IResults results = new Results();
        Product product = new Product
        {
            Id = 17,
            Label = "Amortisseurs"
        };
        products.Add(product);
        product = new Product
        {
            Id = 34,
            Label = "Attelage"
        };
        products.Add(product);
        product = new Product
        {
            Id = 24,
            Label = "Ampoules / lampes"
        };
        products.Add(product);
        product = new Product
        {
            Id = 31,
            Label = "Autoradio"
        };
        products.Add(product);

        return products;
    }

    [WebMethod]
    public static List<Product> GetProductsBySegment(long segmentId)
    {      
        IResults results = new Results();
        return results.GetProductsBySegment(segmentId);
    }

    [WebMethod]
    public static List<Brand> GetBrands()
    {

        List<Brand> brands = new List<Brand>();
        string result = null;
        IResults results = new Results();
        Brand brand = new Brand
        {
            Id = 1,
            Label = "Autobacs"
        };
        brands.Add(brand);
        brand = new Brand
        {
            Id = 2,
            Label = "AD Réparateurs / Autodistribution"
        };
        brands.Add(brand);
        brand = new Brand
        {
            Id = 3,
            Label = "Etape Auto"
        };
        brands.Add(brand);
        brand = new Brand
        {
            Id = 4,
            Label = "Feu Vert"
        };
        brands.Add(brand);

        return brands;
    }
}