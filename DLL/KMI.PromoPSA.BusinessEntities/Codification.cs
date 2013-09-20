using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.BusinessEntities.Classification;

namespace KMI.PromoPSA.BusinessEntities
{
    [Serializable]
    public class Codification
    {
        private Advert _advert;
        private List<Brand> _brands;

        private List<Product> _products ;

        private List<Segment> _segments ;

        private long _currentSegment;

        private List<Product> _currentProducts;

        private long _currentProduct;

        private long _currentBrand;

        public List<Brand> Brands
        {
            get { return _brands; }
            set { _brands = value; }
        }

        public List<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }

        public List<Segment> Segments
        {
            get { return _segments; }
            set { _segments = value; }
        }

        public long CurrentSegment
        {
            get { return _currentSegment; }
            set { _currentSegment = value; }
        }

        public long CurrentProduct
        {
            get { return _currentProduct; }
            set { _currentProduct = value; }
        }

        public long CurrentBrand
        {
            get { return _currentBrand; }
            set { _currentBrand = value; }
        }

        public Advert Advert
        {
            get { return _advert; }
            set { _advert = value; }
        }

        public List<Product> CurrentProducts
        {
            get { return _currentProducts; }
            set { _currentProducts = value; }
        }
    }
}
