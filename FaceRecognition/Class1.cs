using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition
{

    //string path = @"C:\Users\Eimantas\Desktop\Eimantas\20171223_163303.jpg";
    //new Program().RecognitionFace("1", path);
    public class Class1
    {
        FaceServiceClient faceServiceClient = new FaceServiceClient("d96e2f6782b7472684573750c3f41d90", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0");

        public async void CreatePerson(string personId, string personName)
        {
            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personId, personName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Creating person group " + ex.Message);
            }
        }

        public async void AddPerson(string personId, string personName, string path)
        {
            try
            {
                await faceServiceClient.GetPersonGroupAsync(personId);
                CreatePersonResult person = await faceServiceClient.CreatePersonAsync(personId, "Eimantas");

                DetectFaceAndRegister(personId, person, path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error add person " + ex.Message);
            }
        }

        private async void DetectFaceAndRegister(string personId1, CreatePersonResult person, string path)
        {
            foreach (var imgPath in Directory.GetFiles(path, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imgPath))
                {
                    await faceServiceClient.AddPersonFaceAsync(personId1, person.PersonId, s);
                }
                Console.WriteLine("face added");
            }
        }

        public async void TraininAi(string personID)
        {
            await faceServiceClient.TrainPersonGroupAsync(personID);
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personID);
                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }
                await Task.Delay(1000);
                Console.Write(".");
            }
            Console.WriteLine("Training AI completed");
        }

        public async Task<string> RecognitionFace(string personGroupId, string imgPAth)
        {
            using (Stream s = File.OpenRead(imgPAth))
            {
                var face = await faceServiceClient.DetectAsync(s);
                var faceIds = face.Select(x => x.FaceId).ToArray();
                try
                {
                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                    foreach (var identifyresult in results)
                    {
                        Console.WriteLine("Result of face : ", identifyresult.FaceId);
                        if (identifyresult.Candidates.Length == 0)
                        {
                            Console.WriteLine("Not find");
                            return "Not Find";
                        }
                        else
                        {
                            var candidateid = identifyresult.Candidates.First().PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateid);
                            Console.WriteLine("Identify as" + person.Name);
                            return person.Name;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error recognizing face " + ex.Message);
                    return "Error";
                }
                return "Nerastas";
            }
        }
    }
}
