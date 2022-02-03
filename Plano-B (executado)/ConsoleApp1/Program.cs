﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
namespace ConsoleApp1
{
    class Program
    {
        // Recognition model 3 was released in 2020 May
        const string RECOGNITION_MODEL3 = RecognitionModel.Recognition03;

        //const string API_KEY = "3e167dc8783c49c599c391ba7378ec68";
        //const string ENDPOINT = "https://senaifacial-g3.cognitiveservices.azure.com/";
        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
        private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 2 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            Console.WriteLine($"{detectedFaces.Count} face(s) detectada(s) na imagem `{Path.GetFileName(url)}`");
            return detectedFaces.ToList();
        }
        public static async Task FindSimilar(IFaceClient client, string recognition_model)
        {
            Console.WriteLine("========Achar Similares========");
            Console.WriteLine();
            Console.WriteLine("Insira o link da face base(links muito grandes podem causar erros):\n");
            string sourceImageFileName = Console.ReadLine();
            Console.WriteLine("Insira o link das possiveis faces similares e digite FIM quando acabar (links muito grandes podem causar erros):\n");
            List<string> targetImageFileNames = new List<string>();
            string aux = "";
            int i = 1;
            do
            {
                Console.WriteLine($"Imagem {i}:\n");
                aux = Console.ReadLine();
                if (aux != "FIM")
                {
                    targetImageFileNames.Add(aux);
                }
                i++;
            } while (aux != "FIM");


            IList<Guid?> targetFaceIds = new List<Guid?>();
            foreach (var targetImageFileName in targetImageFileNames)
            {
                // Detect faces from target image url.
                var faces = await DetectFaceRecognize(client, $"{targetImageFileName}", recognition_model);
                // Add detected faceId to list of GUIDs.
                targetFaceIds.Add(faces[0].FaceId.Value);
            }

            // Detect faces from source image url.
            IList<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{sourceImageFileName}", recognition_model);
            Console.WriteLine();

            // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
            IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);
            i = 1;
            foreach (var similarResult in similarResults)
            {
                Console.WriteLine($"A imagem {i} com o FaceID:{similarResult.FaceId} é similar a imagem base com a confiança: {similarResult.Confidence}.");
                i++;
            }
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            // From your Face subscription in the Azure portal, get your subscription key and endpoint.
            Console.WriteLine("Insira a URL da sua aplicação no Azure:\n");
            string urlServico = Console.ReadLine();
            Console.WriteLine("Insira a chave da sua aplicação no Azure:\n");
            string chaveServico = Console.ReadLine();

            // Authenticate.
            IFaceClient client = Authenticate(urlServico, chaveServico);
            FindSimilar(client, RECOGNITION_MODEL3).Wait();
        }
    }
}
