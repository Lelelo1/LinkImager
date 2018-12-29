using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LinkImager.Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
namespace Testing
{
    [TestClass]
    public class SerializationTests
    {
        /*
        [TestMethod]
        public void Serialize()
        {
            MR.Gestures.AbsoluteLayout absolute = new MR.Gestures.AbsoluteLayout();
            MovableImage project = new MovableImage("branch.jpg");
            MovableImage movableImage = new MovableImage(absolute, project, new Xamarin.Forms.Rectangle(new Point(0, 0), new Size(40, 40)));
            project.children.Add(movableImage);
            MovableImage childImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(200, 300), new Size(120, 120)));
            movableImage.children.Add(childImage);
            MovableImage otherChildImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(80, 100), new Size(70, 60)));
            movableImage.children.Add(otherChildImage);

            Stream stream = File.Open("project.ii", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, project);
            stream.Close();
            Assert.IsTrue(File.Exists("project.ii"));
        }

        [TestMethod]
        public void Deserialize()
        {
            Stream stream = File.Open("project.ii", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            MovableImage project = (MovableImage) formatter.Deserialize(stream);
            stream.Close();

            Assert.IsNotNull(project);
        }
        */
    }
}
