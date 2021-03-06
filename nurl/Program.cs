﻿/// 
/// Author: James Dickson 
/// More tools and contact information: www.wallparse.com
///
/// This application is created in order to get control over http-requests in .net
/// I use it for fuzzing tasks when conducting penetration tests of web applications.
/// Since it is created in .net and have no dependencies on other non-standard libraries
/// it is really simple to call from powershell scripts or load directly as a .net-assembly.
/// 
/// Example:
/// nurl.exe --host www.google.com --port 80 --httprequest "GET / HTTP/1.1\r\nHost: www.whateverhost.com\r\nContent-length: 777\r\n\r\nJAMES"
/// 
/// To load the assembly in powershell
/// 
/// $strPath = c:\absolutepath\to\nurl.exe
/// $assembly = [Reflection.Assembly]::LoadFile($strPath)
/// $nurl = $assembly.CreateInstance("nurl.Nurl")
/// 
/// After this it is simple to use directly using the $nurl object.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesUtility
{
    class Program
    {
        static void printUsage()
        {
            Console.WriteLine("Usage: \n");
            Console.WriteLine("--host <host/ip>");
            Console.WriteLine("--port <port number>");
            Console.WriteLine("--request <input file>");
            Console.WriteLine("--out <output file>");
            Console.WriteLine("--ssl - Use SSL for HTTPS connections");
            Console.WriteLine("--skipheaders - If headers should be skipped");
            Console.WriteLine("--skipsent - If sent http-request should be skipped");
            Console.WriteLine("--skipadjust - If we should NOT try to adjust the content-length");
            Console.WriteLine("--binary - Do not try to interpret HTTP-headers");
            Console.WriteLine("--sendtimeout - Send-timeout for client");
            Console.WriteLine("--receivetimeout - Receive-timeout for client");
            Console.WriteLine("--replace <replace> <newtext> - Replace a text with another before send");
            Console.WriteLine("--append - If the output file should be opened for append (not reset)");
            Console.WriteLine("--tamperconvert <file> - Convert tamper data copied format into proper request");
            Console.WriteLine("--httprequest <string> - Use httprequest directly from command line");
            Console.WriteLine("--stdin - read from stdin instead.");

        }


        /// <summary>
        /// Here we go
        /// </summary>
        /// <param name="args">See argument parsing loop and printUsage()</param>
        static void Main(string[] args)
        {
            DNurl nurl = new DNurl();
            string strTamperConvertMethod = "GET";
            string strTamperConvertPath = "/";
            bool bActionDone = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--ssl")
                {
                    nurl.bIsSSL = true;
                }
                else if (args[i] == "--binary")
                {
                    nurl.bBinary = true;
                }
                else if (args[i] == "--run")
                {
                    nurl.run();
                }
                else if (args[i] == "--stdin")
                {
                    nurl.bReadFromStdin = true;
                }
                else if (args[i] == "--resprint")
                {
                    string strAsciiResults = nurl.getServerAsciiResults();
                    Console.WriteLine(strAsciiResults);
                }
                else if (args[i] == "--headerprint")
                {
                    System.Collections.Hashtable ht = nurl.getServerHeaders();

                    Console.WriteLine(ht.Count);
                }
                else if (args[i] == "--bodyprint")
                {

                    Console.WriteLine(nurl.getServerAsciiBody());
                }
                else if (args[i] == "--skipheaders")
                {
                    nurl.bSkipHeaders = true;
                }
                else if (args[i] == "--skipsent")
                {
                    nurl.bEchoWrite = false;
                }
                else if (args[i] == "--nogzip")
                {
                    nurl.bDecodeGzip = false;
                }
                else if (args[i] == "--parselength")
                {
                    nurl.bUseContentLength = true;
                }
                else if (args[i] == "--skipadjust")
                {
                    nurl.bParseContentLength = false; // False if we should not adjust the supplied length.
                }
                else if (args[i] == "--append")
                {
                    nurl.bAppend = true;
                }
                else if (args.Length > (i + 1))
                {
                    if (args[i] == "--request")
                    {
                        i++;
                        nurl.strFileName = args[i];
                    }
                    else if (args[i] == "--tamperconvert")
                    {
                        i++;
                        FileConvert.printTamperRequest(args[i], strTamperConvertMethod, strTamperConvertPath);
                        bActionDone = true;
                    }
                    else if (args[i] == "--port")
                    {
                        i++;
                        nurl.port = Convert.ToInt32(args[i]);
                    }
                    else if (args[i] == "--httprequest")
                    {
                        i++;
                        nurl.strHttpRequest = args[i];
                    }
                    else if (args[i] == "--sendtimeout")
                    {
                        i++;
                        nurl.sendTimeout = Convert.ToInt32(args[i]);
                    }
                    else if (args[i] == "--receivetimeout")
                    {
                        i++;
                        nurl.receiveTimeout = Convert.ToInt32(args[i]);
                    }
                    else if (args[i] == "--host")
                    {
                        i++;
                        nurl.strHost = args[i];
                    }
                    else if (args[i] == "--replace")
                    {
                        nurl.strReplacers.Add(args[++i]);
                        nurl.strReplacers.Add(args[++i]);
                    }
                    else if (args[i] == "--out")
                    {
                        i++;
                        nurl.strOutfile = args[i];
                    }
                    else
                    {
                        Console.WriteLine("Bad input argument: " + args[i]);
                        printUsage();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Bad input argument: " + args[i]);
                    printUsage();
                    return;
                }
            }

            if (nurl.strHost == null && !bActionDone)
            {
                printUsage();
                return;
            }

            if (nurl.strHost != null)
            {
                nurl.run();
                nurl.closeOutputStreams();
            }
        }
    }
}
