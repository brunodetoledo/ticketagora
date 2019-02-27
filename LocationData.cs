using System.Net;

namespace TA.Location
{
    public class LocationData
    {
        public string Address { get; set; }
        public string FormattedAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string RegionCode
        {
            get
            {
                switch (this.Region)
                {
                    case "Acre":
                        return "AC";

                    case "Alagoas":
                        return "AL";

                    case "Amapá":
                        return "AP";

                    case "Amazonas":
                        return "AM";

                    case "Bahia":
                        return "BA";

                    case "Ceará":
                        return "CE";

                    case "Distrito Federal":
                        return "DF";

                    case "Espírito Santo":
                        return "ES";

                    case "Goiás":
                        return "GO";

                    case "Maranhão":
                        return "MA";

                    case "Mato Grosso do Sul":
                        return "MS";

                    case "Mato Grosso":
                        return "MT";

                    case "Minas Gerais":
                        return "MG";

                    case "Pará":
                        return "PA";

                    case "Paraíba":
                        return "PB";

                    case "Paraná":
                        return "PR";

                    case "São Paulo":
                        return "SP";

                    case "Pernambuco":
                        return "PE";

                    case "Piauí":
                        return "PI";

                    case "Rio de Janeiro":
                        return "RJ";

                    case "Rio Grande do Norte":
                        return "RN";

                    case "Rio Grande do Sul":
                        return "RS";

                    case "Rôndonia":
                        return "RO";

                    case "Roraima":
                        return "RR";

                    case "Santa Catarina":
                        return "SC";

                    case "Sergipe":
                        return "SE";

                    case "Tocantins":
                        return "TO";

                    default:
                        return string.Empty;
                }
            }
        }
        public string Country { get; set; }
        public string PostalCode { get; internal set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public IPAddress IpAddress { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(FormattedAddress))
            {
                return string.Format("{0} - {1}", FormattedAddress, Country);
            }

            return string.Format("{0}, {1} - {2}, {3} - {4}", Address, City, RegionCode, PostalCode, Country);
        }
    }
}
