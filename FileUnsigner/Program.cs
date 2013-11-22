using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace FileUnsigner
{
    class Program
    {
        [DllImport("Imagehlp.dll ")]
        private static extern bool ImageRemoveCertificate(IntPtr handle, int index);
        static bool forceUnsign = false;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("\n\r---------------------------------------------------------------");
                Console.WriteLine("RESULTS:");

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/f")
                    {
                        forceUnsign = true;
                        continue;
                    }

                    try
                    {
                        if (!File.Exists(args[i]))
                            throw new FileNotFoundException();

                        UnsignFile(args[i], forceUnsign);

                        Console.WriteLine("\n\r" + i + ": Successfully unsigned " + args[i]);
                    }
                    catch (System.Security.Cryptography.CryptographicException)
                    {
                        Console.WriteLine("\n\r" + i + ": Failed trying to unsign " + args[i] + ". File doesn't appear to be digitally signed.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n\r" + i + ": Failed trying to unsign " + args[i] + ". " + ex.Message);
                    }
                }
                Console.WriteLine("---------------------------------------------------------------");
            }
            else
            {
                ShowCopyrightAndUsageInformation();
            }

            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Press any key to exit.");
            Console.ReadKey();
        }


        /// <summary>
        /// Remove a digital certificate from a file.
        /// </summary>
        /// <param name="file">The path of the file.</param>
        /// <param name="forceUnsign">If forceUnsign is false load the file in a X509Certificate object which will throw a <b>CryptographicException</b> exception if a certificate is not found.</param>
        private static void UnsignFile(string file, bool forceUnsign)
        {
            if (!forceUnsign)
            {
                X509Certificate certificate = new X509Certificate(file);
            }

            using (FileStream fs = new FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
            {
                ImageRemoveCertificate(fs.SafeFileHandle.DangerousGetHandle(), 0);
            }
        }

        /// <summary>
        /// Display the copyright and usage information.
        /// </summary>
        private static void ShowCopyrightAndUsageInformation()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version + " Copyright (C) 2013 Fluxbytes");
            Console.WriteLine("Latest version can be found at: http://www.fluxbytes.com/");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("No files detected. Please use the following syntax or simply drag and drop the files into the program.");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("FileUnsigner.exe <options> <file1> <file2> ...");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Options:");
            Console.WriteLine("  /f \t Forces the program to remove a digital signature even if one is not detected.");
        }
    }
}
