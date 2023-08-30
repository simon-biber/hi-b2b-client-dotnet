﻿/*
 * Copyright 2011 NEHTA
 *
 * Licensed under the NEHTA Open Source (Apache) License; you may not use this
 * file except in compliance with the License. A copy of the License is in the
 * 'license.txt' file, which should be provided with this work.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Cryptography.X509Certificates;
using nehta.mcaR32.ProviderReadReferenceData;
using Nehta.VendorLibrary.Common;

namespace Nehta.VendorLibrary.HI.Sample
{
    /// <summary>
    /// Requirements:
    /// a) Certificate from Medicare Australia with the Key Usage of "Digital Signature", for the purpose
    ///    of signing soap request messages, and establishing TLS connections to the HI Service.
    /// b) The TLS Web Service endpoint URL for the HI service.
    /// c) Details for the client product information (PCIN) - These include a QualifiedId for the product, 
    ///    the product name and version, and the product platform. These are provided by Medicare.
    /// d) User identifier details.
    /// </summary>
    class ProviderReadReferenceDataClientSample
    {
        public (readReferenceDataResponse,Exception,string) Sample()
        {
            // ------------------------------------------------------------------------------
            // Set up
            // ------------------------------------------------------------------------------

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Obtain the certificate by serial number
            X509Certificate2 tlsCert = X509CertificateUtil.GetCertificate(
                "32ED",
                X509FindType.FindBySerialNumber,
                StoreName.My,
                StoreLocation.LocalMachine,
                true
                );

            // The same certificate is used for signing the request.
            // This certificate will be different to TLS cert for some operations.
            X509Certificate2 signingCert = tlsCert;

            // Set up client product information (PCIN)
            // Values below should be provided by Medicare
            ProductType product = new ProductType()
            {
                platform = "Windows 10",     // Can be any value
                productName = "HIPS",                               // Provided by Medicare
                productVersion = "8.3",                         // Provided by Medicare
                vendor = new QualifiedId()
                {
                    id = "HIPS0001",                                       // Provided by Medicare               
                    qualifier = "http://ns.electronichealth.net.au/id/hi/vendorid/1.0"                          // Provided by Medicare
                }
            };

            // Set up user identifier details
            QualifiedId user = new QualifiedId()
            {
                id = "srkb",                                             // User ID internal to your system
                qualifier = "http://chamonix.com.au/id/hips/userid/1.0"    // Eg: http://ns.yourcompany.com.au/id/yoursoftware/userid/1.0
            };

            // Set up user identifier details
            QualifiedId hpio = null;
                
            //    new QualifiedId()
            //{
            //    id = "",                                                // HPIO ID internal to your system
            //    qualifier = "http://<anything>/id/<anything>/hpio/1.0"    // Eg: http://ns.yourcompany.com.au/id/yoursoftware/userid/1.0
            //};


            // ------------------------------------------------------------------------------
            // Client instantiation and invocation
            // ------------------------------------------------------------------------------

            // Instantiate the client
            ProviderReadReferenceDataClient client = new ProviderReadReferenceDataClient(
                new Uri("https://www5.medicareaustralia.gov.au/cert/soap/services/"),
                product,
                user,
                hpio,
                signingCert,
                tlsCert);

            try
            {
                // Invokes the read operation
                readReferenceDataResponse readReferenceDataResponse =
                    client.ReadReferenceData(new string[]
                                    {
                                      //"providerTypeCode", 
                                      //"providerSpecialty",
                                      //"providerSpecialisation",
                                      //"organisationTypeCode",
                                      //"organisationService",
                                      //"organisationServiceUnit",
                                      "operatingSystem",
                                    });
                return (readReferenceDataResponse, null, client.SoapMessages.SoapResponse);
            }
            catch (FaultException fex)
            {
                string returnError = "";
                MessageFault fault = fex.CreateMessageFault();
                if (fault.HasDetail)
                {
                    ServiceMessagesType error = fault.GetDetail<ServiceMessagesType>();
                    // Look at error details in here
                    if (error.serviceMessage.Length > 0)
                        returnError = error.serviceMessage[0].code + ": " + error.serviceMessage[0].reason;
                }

                // If an error is encountered, client.LastSoapResponse often provides a more
                // detailed description of the error.
                return (null, fex, client.SoapMessages.SoapResponse);
            }
            catch (Exception ex)
            {
                // If an error is encountered, client.LastSoapResponse often provides a more
                // detailed description of the error.
                return (null, ex, client.SoapMessages.SoapResponse);
            }
        }
    }
}
