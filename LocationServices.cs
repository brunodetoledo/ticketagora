using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net;

namespace TA.Location
{
    public class LocationServices
    {
        private WebClient webClient;

        public LocationServices()
        {
            this.webClient = new WebClient()
            {
                Encoding = System.Text.Encoding.UTF8
            };
        }

        private void AssurePublicIpAddress(ref IPAddress ipAddress)
        {
            if (ipAddress.IsInternal())
            {
                try
                {
                    //https://api.ipify.org
                    using (WebClient webClient = new System.Net.WebClient())
                    {
                        string json = webClient.DownloadString("https://api.ipify.org/?format=json");

                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json);

                            if (data != null && data.ip != null)
                            {
                                ipAddress = IPAddress.Parse(data.ip);
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Método mais preciso. Realiza reverse geocoding no Bing.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <returns>Classe com os dados de localização.</returns>
        public LocationData ReverseGeocoding(string latitude, string longitude, IPAddress ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrEmpty(longitude))
            {
                throw new ArgumentException();
            }

            LocationData locationData = new LocationData()
            {
                Latitude = latitude,
                Longitude = longitude
            };

            if (ipAddress != null)
            {
                this.AssurePublicIpAddress(ref ipAddress);
                locationData.IpAddress = ipAddress;
            }

            using (webClient)
            {
                string json = webClient.DownloadString(string.Format("http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=json&key=", locationData.Latitude.ToString().Replace(",", "."), locationData.Longitude.ToString().Replace(",", ".")));

                if (!string.IsNullOrWhiteSpace(json))
                {
                    dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(json);

                    try
                    {
                        if (dyn != null && dyn.resourceSets.Count > 0 && dyn.resourceSets[0].resources.Count > 0)
                        {

                            locationData.FormattedAddress = dyn.resourceSets[0].resources[0].address.formattedAddress;
                            locationData.Address = dyn.resourceSets[0].resources[0].address.addressLine;
                            locationData.City = dyn.resourceSets[0].resources[0].address.locality;
                            locationData.Region = dyn.resourceSets[0].resources[0].address.adminDistrict;
                            locationData.Country = dyn.resourceSets[0].resources[0].address.countryRegion;
                            locationData.PostalCode = dyn.resourceSets[0].resources[0].address.postalCode;
                            return locationData;
                        }
                    }
                    catch (Exception) { }
                }
            }


            if (locationData.IpAddress != null)
                locationData = this.IPGeocoding(ipAddress);

            return locationData;
        }

        /// <summary>
        /// Método menos preciso, somente cidade, estado e país. Realiza consulta via uma série de serviços e retorna o primeiro que realizar a consulta com sucesso.
        /// </summary>
        /// <param name="ipAddress">Endereço IP a ser consultado.</param>
        /// <returns>Classe com os dados de localização</returns>
        public LocationData IPGeocoding(IPAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentException();
            }

            this.AssurePublicIpAddress(ref ipAddress);

            LocationData locationData = new LocationData()
            {
                IpAddress = ipAddress
            };

            using (this.webClient)
            {
                string[] accounts = new string[] { "8a1e312478774288b12e7e38a03ce305", "1c1b2d9992f245fa8c49304972ca55b8", "8509de47480c4c4da8f25522e1d37ce0", "b22ec149f52347d8908349565d546ef8", "b22d8e58f4bf4d8c846d5035f75eb309" };

                foreach (var account in accounts)
                {
                    string json = webClient.DownloadString(string.Format("https://api.ipgeolocation.io/ipgeo?apiKey={0}&ip={1}", account, locationData.IpAddress));

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json);

                        try
                        {
                            if (data != null)
                            {
                                locationData.Latitude = data.latitude.ToString();
                                locationData.Longitude = data.longitude.ToString();
                                locationData.City = data.city.ToString();
                                locationData.Region = data.state_prov.ToString();
                                locationData.Country = data.country_name.ToString();

                                return locationData;
                            }
                        }
                        catch
                        {
                            locationData = new LocationData()
                            {
                                IpAddress = ipAddress
                            };
                        }
                    }
                }
            }

            return locationData;
        }
    }
}
