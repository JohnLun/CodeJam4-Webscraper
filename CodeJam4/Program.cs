using HtmlAgilityPack;
using Figgle;
using Colorful;
using System.Drawing;
using System.Web;
using Console = System.Console;

namespace CodeJam4
{
    internal class Program
    {
        public static HtmlDocument LoadUrl(string fileUrl)
        {
            var url = fileUrl;
            var web = new HtmlWeb();
            return web.Load(url);
        }

        public static void GrabHeadline(HtmlDocument doc)
        {
            var title = doc.DocumentNode
                 .SelectSingleNode("//h1[contains(@class, 'article-hero-headline__htag')]");

            if (title != null)
            {
                //Colorful.Console.WriteLine(title.InnerHtml, Color.FromArgb(255, 255, 250));
                Colorful.Console.WriteWithGradient(title.InnerHtml, Color.HotPink, Color.RebeccaPurple, 4);
            }
            else
            {
                Colorful.Console.WriteLine("No title");
            }
        }

        public static void PrintContent(HtmlDocument doc)
        {

            var subTitleNodes = doc.DocumentNode
                .SelectNodes("//h2[contains(@class, 'styles_chartCardHeadline__aiJF0')]");
            var subTitleContentNodes = doc.DocumentNode
                .SelectNodes("//div[@data-icid]");

            List<string> subTitles = new List<string>();
            List<List<string>> subTitleContents = new List<List<string>>();

            //Getting all Sub Headings

            if (subTitles != null)
            {
                foreach (var subtitle in subTitleNodes)
                {
                    //Colorful.Console.WriteLine(HtmlEntity.DeEntitize(subtitle.InnerHtml));
                    subTitles.Add(HtmlEntity.DeEntitize(subtitle.InnerHtml));
                    
                    
                }
            }
            else
            {
                Colorful.Console.WriteLine("No Subtitles Found", Color.PaleVioletRed);
            }
            

            //Getting Content from each subheading.

            if (subTitleContentNodes != null)
            {
                foreach (var subTitleContentNode in subTitleContentNodes)
                {
                    var pNodes = subTitleContentNode.SelectNodes(".//p");
                    
                    if(pNodes != null)
                    {
                        List<string> pNodesContent = new List<string>();
                        foreach (var p in pNodes)
                        {
                            HtmlDocument temp = new HtmlDocument();
                            temp.LoadHtml(p.InnerHtml);
                            string cleanText = temp.DocumentNode.InnerText;
                            cleanText = HttpUtility.HtmlDecode(cleanText);
                            pNodesContent.Add(HtmlEntity.DeEntitize(cleanText));
                        }
                        subTitleContents.Add(pNodesContent);
                    }
                   
                }
            }

            for (int i = 0; i < subTitles.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(subTitles[i], Color.Aquamarine); // prints the subtitle
                Console.ResetColor();
                List<string> contents = subTitleContents[i];
                Colorful.Console.WriteLine("----------------------------------------\n");
                for (int h = 0; h < contents.Count; h++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(contents[h] + "\n", Color.CornflowerBlue);
                    Console.ResetColor();// prints the content within the subtitle
                }
                
                Console.ResetColor();
                Console.WriteLine("----------------------------------------\n");
            }
        }

        static void Main(string[] args)
        {

            //System.Console.BackgroundColor = ConsoleColor.Gray;
            bool vrajMode = false;

            string figletText = FiggleFonts.Standard.Render("NBC News Outliner!");
            Colorful.Console.WriteWithGradient(figletText, Color.LemonChiffon, Color.AliceBlue);

            Console.WriteLine("Enter Vraj Mode? [Y/N]");
            string ans = Console.ReadLine();

            if (ans.Equals('Y'))
            {
                vrajMode = true;
                System.Console.BackgroundColor = ConsoleColor.Red;
            }

            Colorful.Console.WriteLine("Enter the url of a NBC live coverage news article that you want to create an outline of:", Color.Fuchsia);

            string? url = Colorful.Console.ReadLine();
            var doc = LoadUrl(url);

            GrabHeadline(doc);
            
            PrintContent(doc);

        }
    }
}
