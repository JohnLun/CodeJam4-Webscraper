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
            var web = new HtmlWeb();
            try
            {
                return web.Load(fileUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading the URL: " + ex.Message, Color.Red);
                return null;
            }
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

        // Validate the URL format and ensure it contains 'live-blog' and 'nbcnews.com'
        public static bool IsValidUrl(string url)
        {
            bool isValidUri = Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                   
            bool containsRequiredKeywords = url.Contains("live-blog") && url.Contains("nbcnews.com");

            return isValidUri && containsRequiredKeywords;
        }

        static void Main(string[] args)
        {
            bool restartProgram = true;

            while (restartProgram)
            {
            //System.Console.BackgroundColor = ConsoleColor.Gray;
            bool vrajMode = false;

            string figletText = FiggleFonts.Standard.Render("NBC News Outliner!");
            Colorful.Console.WriteWithGradient(figletText, Color.LemonChiffon, Color.AliceBlue);

            string? ans;
            do
            {
                Console.WriteLine("Enter Vraj Mode? [Y/N]");
                ans = Console.ReadLine()?.ToUpper();

                if (ans != "Y" && ans != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' or 'N'.", Color.Red);
                }

            } while (ans != "Y" && ans != "N");

            if (ans.Equals('Y'))
            {
                vrajMode = true;
                System.Console.BackgroundColor = ConsoleColor.Red;
            }

            // URL input and validation
            string url;
            do
            {
                Colorful.Console.WriteLine("Enter the URL of an NBC live coverage news article:", Color.Fuchsia);
                url = Colorful.Console.ReadLine();

                if (!IsValidUrl(url))
                {
                    Console.WriteLine("Invalid URL. Please enter a valid URL (must start with http or https).", Color.Red);
                }

            } while (!IsValidUrl(url));
            var doc = LoadUrl(url);

            GrabHeadline(doc);
            
            PrintContent(doc);// Ask user to restart or quit
                string? restartAns;
                do
                {
                    Console.WriteLine("Would you like to restart or quit? [R/Q]");
                    restartAns = Console.ReadLine()?.ToUpper();

                    if (restartAns != "R" && restartAns != "Q")
                    {
                        Console.WriteLine("Invalid input. Please enter 'R' to restart or 'Q' to quit.", Color.Red);
                    }

                } while (restartAns != "R" && restartAns != "Q");

                if (restartAns == "Q")
                {
                    restartProgram = false;  // Exit the loop to quit the program
                }
                else
                {
                    Console.Clear();  // Clear the console to restart
                }
            }

            Console.WriteLine("Goodbye!");
        }
    }
}