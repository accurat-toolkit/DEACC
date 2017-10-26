using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Multithreading;

namespace cooc_generalizat_forme
{
    class Program
    {

        private static void read_cfg(string p, ref string multithreading, ref int loading, ref int frequency, ref int window, ref int ll, ref string sourceamverblist, ref string targetamverblist, ref string crossPOS, ref string sPOSlist, ref string tPOSlist, ref string LD)
        {
            StreamReader rdr = new StreamReader(p, Encoding.UTF8);
            string line = "";
            while ((line = rdr.ReadLine()) != null)
            {
                if (!line.StartsWith("//") || !line.StartsWith("*") || (line != "")) 
              
                { 
                    string[] infs = line.Split(':'); 
                    if (infs[0]=="multithreading") multithreading = infs[1];
                    if (infs[0]=="loading") loading = int.Parse(infs[1]);
                    if (infs[0]=="frequency") frequency = int.Parse(infs[1]);
                    if (infs[0] == "window") window = int.Parse(infs[1]);
                    if (infs[0] == "ll") ll = int.Parse(infs[1]);
                    if (infs[0] == "sourceamverblist") sourceamverblist = infs[1];
                    if (infs[0] == "targetamverblist") targetamverblist = infs[1];
                    if (infs[0] == "crossPOS") crossPOS = infs[1];
                    if (infs[0] == "sPOSlist") sPOSlist = infs[1];
                    if (infs[0] == "tPOSlist") tPOSlist = infs[1];
                    if (infs[0] == "LD") LD = infs[1];
                }

            }
        }
        public static List<double> sortlist(List<double> list)
        {
            List<double> newlist = new List<double>();
            if (list.Count != 0)
            {
                bool permutare = true;
                do
                {
                    permutare = false;
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        if (list[i] > list[i + 1])
                        {
                            double periodic = list[i + 1];
                            list[i + 1] = list[i];
                            list[i] = periodic;
                            permutare = true;
                        }
                    }
                }
                while (permutare == true);
                // elimina dubluri
                newlist.Add(list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i] != list[i - 1])
                        newlist.Add(list[i]);
                }
            }
            return newlist;
        }
        private static Hashtable numarare_coocurente(string p, ref int count, int window, string POSlist)
        {
            string[] files = Directory.GetFiles(p, "file*");
            string[] poss = POSlist.Split(' ');
            Hashtable cooc = new Hashtable();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    string line = "";
                    StreamReader readfile = new StreamReader(files[i]);
                    while ((line = readfile.ReadLine()) != null)
                    {
                        line = line.ToLower();
                        string[] words = line.Split(' ');
                        count = count + words.Length;
                        int startw = 0; int stopw = 0;
                        if (words.Length > (window-1))
                        { stopw = window-1; }
                        else
                        { stopw = words.Length - 1; }
                        while (startw != words.Length - 1)
                        {
                            string[] startinfs = words[startw].Split('^');
                            if (startinfs[1].StartsWith(poss[0]) || startinfs[1].StartsWith(poss[1]) || startinfs[1].StartsWith(poss[2]) || startinfs[1].StartsWith(poss[3]) || startinfs[1].StartsWith(poss[4]))
                            {
                                for (int j = startw + 1; j <= stopw; j++)
                                {

                                    string[] infs = words[j].Split('^');
                                    if (infs[1].StartsWith(poss[0]) || infs[1].StartsWith(poss[1]) || infs[1].StartsWith(poss[2]) || infs[1].StartsWith(poss[3]) || infs[1].StartsWith(poss[4]))
                                    {
                                        if (!cooc.ContainsKey(startinfs[0] + " " + infs[0])) cooc.Add(startinfs[0] + " " + infs[0], 1);
                                        else cooc[startinfs[0] + " " + infs[0]] = int.Parse(cooc[startinfs[0] + " " + infs[0]].ToString()) + 1;
                                    }
                                    else
                                    { }
                                }
                            }
                            else 
                            { }
                            startw++;
                            
                            if (stopw < words.Length - 1) stopw++;
                        }
                    }
                }
                catch { break; }
            }
            return cooc;
        }
        public static Dictionary<string, int> extrage_detradus(string folder, ref Dictionary<string, int> lexical_freq, string POSlist, string amverblist)
        {
            string[] poss = POSlist.Split(' ');
            string[] amvs = amverblist.Split(' ');
            Dictionary<string, int> wind = new Dictionary<string, int>();
            string[] tfiles = Directory.GetFiles(folder, "*_corpus.txt");
            int totalcontor = 0;
            for (int f = 0; f < tfiles.Length; f++)
            {
                StreamReader rcorpus = new StreamReader(tfiles[f], Encoding.UTF8);

                int contor = 0;
                string line = "";
                while ((line = rcorpus.ReadLine()) != null)
                {

                    line = line.Replace("\u0219", "\u015F");
                    line = line.Replace("\u0218", "\u015E");
                    line = line.Replace("\u021A", "\u0162");
                    line = line.Replace("\u021B", "\u0163");
                    int wcontor = 0;
                    contor++;
                    line = line.TrimEnd();
                    line = line.ToLower();
                    string[] words = line.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        bool am = false;
                        wcontor++;
                        totalcontor++;
                        string[] infos = words[i].Split('|');
                        if (infos[1].Contains("^"))
                        {
                            string[] ws = infos[1].Split('^');
                            for (int j = 0; j < amvs.Length; j++)
                            {
                                if (ws[0] == amvs[j]) am = true;
                            }
                            string word = infos[0].ToLower();
                            if (!lexical_freq.ContainsKey(word))
                            {
                                lexical_freq.Add(word, 1);
                            }
                            else lexical_freq[word] = lexical_freq[word] + 1;
                            string msd = infos[3];
                            if ((am != true) && (word.Length > 1))
                            {
                                if (msd.StartsWith(poss[0]) || msd.StartsWith(poss[1]) || msd.StartsWith(poss[2]) || msd.StartsWith(poss[3]) || msd.StartsWith(poss[4]))
                                {
                                    if (wind.ContainsKey(word + "^" + infos[3].Substring(0, 2)))
                                    {
                                        wind[word + "^" + infos[3].Substring(0, 2)] = wind[word + "^" + infos[3].Substring(0, 2)] + 1;
                                    }
                                    else wind[word + "^" + infos[3].Substring(0, 2)] = 1;
                                }
                            }
                           
                        }
                        else
                        {
                            string word = infos[0].ToLower();
                            if (!lexical_freq.ContainsKey(word))
                            {
                                lexical_freq.Add(word, 1);
                            }
                            else lexical_freq[word] = lexical_freq[word] + 1;
                           
                                string msd = infos[3];
                                for (int j = 0; j < amvs.Length; j++)
                                {
                                    if (word == amvs[j]) am = true;
                                }
                               if ((am != true) && (word.Length > 1))
                                {
                                    if (word == "g8")
                                    {
                                    }
                                   if (msd.StartsWith(poss[0]) || msd.StartsWith(poss[1]) || msd.StartsWith(poss[2]) || msd.StartsWith(poss[3])||msd.StartsWith(poss[4]))
                                    {
                                        if (wind.ContainsKey(word + "^" + infos[3].Substring(0, 2)))
                                        {
                                            wind[word + "^" + infos[3].Substring(0, 2)] = wind[word + "^" + infos[3].Substring(0, 2)] + 1;
                                        }
                                        else wind[word + "^" + infos[3].Substring(0, 2)] = 1;
                                    }
                                }
                        }
                    }
                }
                rcorpus.Close();
            }
            return wind;
        }

        public static void calcul_log_direct(string cooc, Dictionary<string, int> wind, Hashtable dict, long corpus_count, string log_output, Dictionary<string, int> lex_freq)
        {
            StreamReader vcooc = new StreamReader(cooc, Encoding.UTF8);
            StreamWriter loglikelyhood = new StreamWriter(log_output, false, Encoding.UTF8);
            string line = "";
            while ((line = vcooc.ReadLine()) != null)
            {
                line = line.Substring(0, line.Length-1);
                string[] infs = line.Split('|');
                string pivot = infs[0].Trim();
                loglikelyhood.Write(infs[0] + "|");
                Hashtable cooccount = new Hashtable();
                for (int i = 1; i < infs.Length; i++)
                {
                    string[] spl = infs[i].Split('*');
                    string k = spl[0];
                    if (!cooccount.ContainsKey(k))
                    {
                        cooccount.Add(k, int.Parse(spl[1]));
                    }
                    else
                    {
                        cooccount[k] = int.Parse(cooccount[k].ToString()) + int.Parse(spl[1]);
                    }
                }
                foreach (string key in cooccount.Keys)
                {
                    double k11 = 0; double k12 = 0; double k21 = 0; double k22 = 0; double C1 = 0; double C2 = 0;
                    double R1 = 0; double R2 = 0; ; double N = 0;
                    if (dict.ContainsKey(key))
                    {

                        k11 = double.Parse(cooccount[key].ToString());
                        double trfreq = (double)wind[pivot];
                        int dictfreq = lex_freq[key];
                        k12 = Math.Abs(trfreq - k11);
                        k21 = Math.Abs(dictfreq - k11);
                        //length = lungimea corpusului
                        k22 = corpus_count - trfreq - dictfreq;
                        C1 = k11 + k12;
                        C2 = k21 + k22;
                        R1 = k11 + k21;
                        R2 = k12 + k22;
                        N = k11 + k12 + k21 + k22;

                        double l1 = Math.Log(((double)k11 * N) / ((double)C1 * R1));
                        double l2 = 0; double l3 = 0;
                        if (k12 != 0)
                        {
                            l2 = Math.Log(((double)k12 * N) / ((double)C1 * R2));
                        }
                        if (k21 != 0)
                        {
                            l3 = Math.Log(((double)k21 * N) / ((double)C2 * R1));
                        }
                        double l4 = Math.Log(((double)k22 * N) / ((double)C2 * R2));
                        double ll = k11 * l1 + k12 * l2 + k21 * l3 + k22 * l4;
                        if (!double.IsNaN(ll))
                        { loglikelyhood.Write("{0}*{1:f4}|", key, ll); }
                        else
                        { }
                    }
                }
                loglikelyhood.WriteLine();

            }
            loglikelyhood.Close();

        }

        public static Hashtable load_dictionary(string dictionary, string direction)
        {

            string line = "";
            StreamReader dictionar = new StreamReader(dictionary, Encoding.UTF8);
            string prefix = "";
            if (direction == "target")
            {
                prefix = "t";
            }
            else
            {
                prefix = "s";
            }
            StreamWriter wrtde_tradus = new StreamWriter(prefix + "detradus", false, Encoding.UTF8);
            Hashtable dict = new Hashtable();
            int cline = 0;
            while ((line = dictionar.ReadLine()) != null)
            {
                cline++;
                line = line.Replace(" ||| ", "|");
                string[] infs = line.Split('|');
                bool to_add = true;

                string[] scoruri = infs[4].Trim().Split(' ');
                string scor_de_adaugat = "";
                if (direction == "target")
                { scor_de_adaugat = scoruri[0]; }
                else { scor_de_adaugat = scoruri[2]; }
                if (to_add == true)
                {
                    string key = "";
                    if (direction == "target")
                    {
                        key = infs[1].ToLower();
                    }
                    else
                    {
                        key = infs[0].ToLower();
                    }
                    if (!dict.ContainsKey(key.Trim()))
                    {
                        dict.Add(key.Trim(), cline + "_" + scor_de_adaugat);
                    }
                    else dict[key] = dict[key] + " " + cline + "_" + scor_de_adaugat;
                }
            }
            return dict;
        }
        private static void calcul_vectori_ulterior(Hashtable tdict, Hashtable sdict)
        {
            StreamReader target = new StreamReader("log_word_target.f", Encoding.UTF8);
            StreamReader source = new StreamReader("log_word_source.f", Encoding.UTF8);

            StreamWriter svector = new StreamWriter("source_vectors", false, Encoding.UTF8);
            StreamWriter tvector = new StreamWriter("target_vectors", false, Encoding.UTF8);



            string line = "";

            while ((line = source.ReadLine()) != null)
            {
                Hashtable soc = new Hashtable();
                line = line.TrimEnd();
                line = line.Substring(0, line.Length - 1);
                string[] sourcecooc = line.Split('|');
                string word = sourcecooc[0].Trim();
                for (int i = 1; i < sourcecooc.Length; i++)
                {

                    string to_split = sourcecooc[i].TrimEnd();
                    string[] tspl = to_split.Split('*');
                    string cheie = tspl[0];
                    cheie = cheie.Trim();
                    if (sdict.ContainsKey(cheie))
                    {
                        string trads = sdict[cheie].ToString();
                        string[] trsplit = trads.Split(' ');
                        for (int t = 0; t < trsplit.Length; t++)
                        {
                            string[] infst = trsplit[t].Split('_');
                            soc.Add(infst[0], Double.Parse(infst[1]) * Double.Parse(tspl[1])); // 
                        }

                    }

                }

                svector.Write(word + "|");
                foreach (string key in soc.Keys)
                {
                    svector.Write("{0}_{1:f4}|", key, soc[key]);
                    svector.Flush();
                }
                svector.WriteLine();
            }
            svector.Close();
            while ((line = target.ReadLine()) != null)
            {
                Hashtable toc = new Hashtable();
                line = line.TrimEnd();
                line = line.Substring(0, line.Length - 1);
                string[] targetcooc = line.Split('|');
                string word = targetcooc[0].Trim();
                for (int i = 1; i < targetcooc.Length; i++)
                {

                    string to_split = targetcooc[i].TrimEnd();
                    string[] tspl = to_split.Split('*');
                    string cheie = tspl[0];
                    cheie = cheie.Trim();
                    if (tdict.ContainsKey(cheie))
                    {
                        string trads = tdict[cheie].ToString();
                        string[] trsplit = trads.Split(' ');
                        for (int t = 0; t < trsplit.Length; t++)
                        {
                            string[] infst = trsplit[t].Split('_');
                            toc.Add(infst[0], Double.Parse(infst[1]) * Double.Parse(tspl[1])); //Double.Parse(infst[1]) *
                        }

                    }

                }

                tvector.Write(word + "|");
                foreach (string key in toc.Keys)
                {
                    tvector.Write("{0}_{1:f4}|", key, toc[key]);
                    //svector.Write(key +" " + sourcec[key]);
                    tvector.Flush();
                }
                tvector.WriteLine();
            }
            tvector.Close();
        }
        private static void separare_pos(string f)
        {
            StreamReader whole_file = new StreamReader(f, Encoding.UTF8);
            string[] infs = f.Split('_');
            StreamWriter verbs = new StreamWriter("v" + infs[0], false, Encoding.UTF8);
            StreamWriter nounsc = new StreamWriter("nc" + infs[0], false, Encoding.UTF8);
            StreamWriter nounsp = new StreamWriter("np" + infs[0], false, Encoding.UTF8);
            StreamWriter adjs = new StreamWriter("a" + infs[0], false, Encoding.UTF8);
            StreamWriter adv = new StreamWriter("r" + infs[0], false, Encoding.UTF8);
            string line = "";
            while ((line = whole_file.ReadLine()) != null)
            {
                string[] fspl = line.Split('|');
                string[] sspl = fspl[0].Split('^');
                if (sspl[1].StartsWith("r"))
                {
                    adv.WriteLine(line);
                }
                if (sspl[1].StartsWith("n"))
                {
                    if (sspl[1].StartsWith("nc"))
                        nounsc.WriteLine(line);
                    else nounsp.WriteLine(line);
                }
                else
                {
                    {
                        if (sspl[1].StartsWith("v"))
                        {
                            verbs.WriteLine(line);
                        }
                        else adjs.WriteLine(line);
                    }
                }
            }
            verbs.Close(); nounsc.Close(); nounsp.Close(); adjs.Close(); whole_file.Close(); adv.Close();
        }
        private static int LevenshteinDistance(String s, String t)
        {
            int ret = 0;

            if (s == null || t == null)
            {
                Console.WriteLine("Strings must not be null");
            }

            int n = s.Length; // length of s
            int m = t.Length; // length of t

            if (n == 0)
            {
                ret = m;
            }
            else if (m == 0)
            {
                ret = n;
            }
            else
            {
                int[] p = new int[n + 1]; // 'previous' cost array, horizontally
                int[] d = new int[n + 1]; // cost array, horizontally
                int[] _d; // placeholder to assist in swapping p and d

                // indexes into strings s and t
                int i; // iterates through s
                int j; // iterates through t

                char t_j; // jth character of t

                int cost; // cost

                for (i = 0; i <= n; i++)
                {
                    p[i] = i;
                }

                for (j = 1; j <= m; j++)
                {
                    t_j = t[j - 1];
                    d[0] = j;

                    for (i = 1; i <= n; i++)
                    {
                        cost = s[i - 1] == t_j ? 0 : 1;
                        // minimum of cell to the left+1, to the top+1, diagonally
                        // left
                        // and up +cost
                        d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1]
                                + cost);
                    }

                    // copy current distance counts to 'previous row' distance
                    // counts
                    _d = p;
                    p = d;
                    d = _d;
                }

                // our last action in the above loop was to switch d and p, so p now
                // actually has the most recent cost counts
                ret = p[n];
            }

            return ret;

        } //int computeLevenshteinDistance(String s, String t)

        public static void basic_similarity_multithread(string source, string target, string pos, int howMany, string LD)
        {
            StreamWriter asocieri = new StreamWriter("dictionary" + pos, false, Encoding.UTF8);
            asocieri.AutoFlush = true;
            StreamReader svectori = new StreamReader(source, Encoding.UTF8);
            int contor = 1;

            Hashtable shash = new Hashtable();
            Hashtable thash = new Hashtable();
            string line = "";
            Dictionary<string, double> tscoruri = new Dictionary<string, double>();

            while ((line = svectori.ReadLine()) != null)
            {
                thash.Clear();
                string[] sinfs = line.Split(new char[] { '|' }, 2);
                if (sinfs[0].Length > 0)
                {
                    StreamReader tvectori = new StreamReader(target, Encoding.UTF8);
                    int tcontor = 0;
                    string[] tinfs = new string[0];
                    while ((line = tvectori.ReadLine()) != null)
                    {
                        tinfs = line.Split(new char[] { '|' }, 2);
                        tcontor++;
                        if (tcontor % howMany != 0)
                        {
                            if ((tinfs[0].Length > 3) && (tinfs[1] != ""))
                            {
                                thash.Add(tinfs[0], tinfs[1]);
                            }
                        }
                        else
                        {
                            addToThash(tinfs, ref thash, ref tscoruri, sinfs[1]);
                        }
                    }

                    if (thash.Count > 0)
                    {
                        addToThash(tinfs, ref thash, ref tscoruri, sinfs[1]);
                    }

                    tvectori.Close();
                    double[] vals;
                    string[] keys;
                    if ((pos == "np") && (LD == "yes"))
                    {
                        Dictionary<string, double> ldtscoruri = new Dictionary<string, double>();
                        foreach (string key in tscoruri.Keys)
                        {
                            int com = LevenshteinDistance(sinfs[0], key);
                            double dcom = (double)com;
                            if (dcom == 0) { dcom = 1; }
                            double sim = tscoruri[key];
                            if ((double)(dcom / (double)Math.Min(sinfs[0].Length, key.Length)) < 0.3) //?
                            {
                                sim = tscoruri[key] / ((double)dcom / (double)Math.Min(sinfs[0].Length-3, key.Length-3));
                                if (sim >= 1)
                                {
                                    sim = 0.99;
                                }
                            }
                            ldtscoruri.Add(key, sim);
                        }
                        vals = ldtscoruri.Values.ToArray();
                        keys = ldtscoruri.Keys.ToArray();
                    }
                    else
                    {
                        vals = tscoruri.Values.ToArray();
                        keys = tscoruri.Keys.ToArray();
                    }
                    Array.Sort(vals, keys);
                    asocieri.Write(sinfs[0] + "|");
                    int min = (keys.Length > 10) ? keys.Length - 11 : 0;

                    for (int i = keys.Length - 1; i >= min; i--)
                    {
                        asocieri.Write(keys[i] + " " + vals[i] + "#");
                        
                    }

                    asocieri.WriteLine();

                    Console.WriteLine(contor++ + " " + sinfs[0]);
                }
                tscoruri.Clear();
            }
            svectori.Close();
            asocieri.Close();
        }

        private static void addToThash(string[] tinfs, ref Hashtable thash, ref Dictionary<string, double> tscoruri, string sinfs_i)
        {
            Dictionary<string, double> scoruri = Multithreader.fork(sinfs_i, thash, dice_Min);
            foreach (string key in scoruri.Keys)
            {
                if (scoruri[key] != 0)
                {
                    tscoruri.Add(key, scoruri[key]);
                }
            }
            thash.Clear();
            if ((tinfs[0].Length > 3) && (tinfs[1] != ""))
            {
                thash.Add(tinfs[0], tinfs[1]);
            }
        }

        public static void basic_similarity(string source, string target, string pos, string LD)
        {
            StreamWriter asocieri = new StreamWriter("asocieri" + pos, false, Encoding.UTF8);
            StreamReader svectori = new StreamReader(source, Encoding.UTF8);
            Hashtable toc = new Hashtable();
            Hashtable soc = new Hashtable();
            string line = "";
            char[] separator = new char[1];
            separator[0] = '|';
            int contor = 0;
            while ((line = svectori.ReadLine()) != null)
            {
                List<double> minfive = new List<double>();
                Hashtable candidati = new Hashtable();
                string[] sinfs = line.Split(separator, 2);
                StreamReader tvectori = new StreamReader(target, Encoding.UTF8);
                while ((line = tvectori.ReadLine()) != null)
                {

                    string[] tinfs = line.Split(separator, 2);
                    if ((tinfs[0].Length > 4) && (tinfs[1] != ""))
                    {
                        double sim = dice_Min(sinfs[1], tinfs[1]);
                        string sform = sinfs[0].Substring(0, sinfs[0].Length - 3);
                        string tform = tinfs[0].Substring(0, tinfs[0].Length - 3);
                        if ((pos == "np") && (LD == "yes"))
                        {
                            int com = LevenshteinDistance(sinfs[0], tinfs[0]);
                            double dcom = (double)com;
                            if (dcom == 0) { dcom = 0.1; }
                            if ((double)(dcom / (double)Math.Min(sinfs[0].Length, tinfs[0].Length)) < 0.3)
                            {
                                sim = sim / ((double)dcom / (double)Math.Min(sinfs[0].Length, tinfs[0].Length));
                            }
                        }
                        if (sim != 0)
                        {
                            minfive.Add(sim);
                            if (!candidati.ContainsKey(sim))
                            {
                                candidati.Add(sim, tinfs[0]);
                            }
                            else
                            {
                                candidati[sim] = candidati[sim] + " " + tinfs[0];
                            }
                            if (minfive.Count > 10)
                            {
                                sortlist(minfive);
                                candidati.Remove(minfive[0]);

                                minfive.RemoveAt(0);

                            }
                        }


                    }

                }
                asocieri.Write(sinfs[0] + "|");
                for (int i = minfive.Count - 1; i >= 0; i--)
                {
                    asocieri.Write(candidati[minfive[i]].ToString() + " " + minfive[i] + "#");
                }

                asocieri.WriteLine();
                asocieri.Flush();
                Console.WriteLine(contor++ + " " + sinfs[0]);
                tvectori.Close();
            }

            svectori.Close();
            asocieri.Close();


        }
        public static void splitare_documente_msd(string cale)
        {
            
            string[] tfiles = Directory.GetFiles(cale, "*_corpus.txt");
            string line = "";
            StreamWriter filewrt = new StreamWriter(cale + "\\filemodified", false, Encoding.UTF8);
            for (int i = 0; i < tfiles.Length; i++)
            {
                StreamReader tcorpus = new StreamReader(tfiles[i], Encoding.UTF8);
             
                while ((line = tcorpus.ReadLine()) != null)
                {
                    line = line.Trim();
                    line = line.Replace("\u0219", "\u015F");
                    line = line.Replace("\u0218", "\u015E");
                    line = line.Replace("\u021A", "\u0162");

                    string[] infos = line.Split(' ');
                    string to_write = "";
                    for (int f = 0; f < infos.Length; f++)
                    {

                        if (!infos[f].Contains("|||"))
                        {
                            string[] wrd = infos[f].Split('|');

                            if (!(wrd[1]).Contains("^"))
                            {
                                if ((wrd.Length > 2) && (wrd[2].Length > 1))
                                    to_write = to_write + wrd[0] + "^" + wrd[2].Substring(0, 2) + " ";
                                if ((wrd.Length > 2) && !(wrd[2].Length > 1))
                                    to_write = to_write + wrd[0] + "^" + wrd[2].Substring(0, 1) + " "; ;
                                if (!(wrd.Length > 2)) to_write = to_write + wrd[0] + " ";
                            }
                            else
                            {
                                if ((wrd.Length > 2) && (wrd[2].Length > 1))
                                    to_write = to_write + wrd[0] + " ";
                                if ((wrd.Length > 2) && !(wrd[2].Length > 1))
                                    to_write = to_write + wrd[0] + "^" + wrd[2].Substring(0, 1) + " "; ;
                                if (!(wrd.Length > 2)) to_write = to_write + wrd[0] + " ";

                            }
                        }
                    }
                    to_write = to_write.TrimEnd();
                    filewrt.WriteLine(to_write);

                }

                tcorpus.Close();
            }
            filewrt.Close();

        }
        private static double dice_Min(string sinf, string tinf)
        {

            string[] target = tinf.Split('|');
            string[] source = sinf.Split('|');

            if (target.Length < source.Length)
            {
                return dice_Min(target, source);
            }
            return dice_Min(source, target);
        }

        private static double dice_Min(string[] source, string[] target)
        {
            double dist = 0;
            double min = 0;

            Dictionary<string, double> soc = new Dictionary<string, double>();
            double cardsoc = 0; //cardinal de soc
            for (int i = 0; i < source.Length - 1; i++)
            {
                string[] infs = source[i].Split('_');
                double x2 = Double.Parse(infs[1]);
                soc.Add(infs[0], x2);
                cardsoc = cardsoc + x2;
            }

            double cardtoc = 0; //cardinal de toc
            for (int i = 0; i < target.Length - 1; i++)
            {
                string[] infs = target[i].Split('_');
                string key = infs[0];
                double x1 = Double.Parse(infs[1]);
                cardtoc = cardtoc + x1;

                if (soc.ContainsKey(key))
                {
                    min = min + Math.Min(x1, soc[key]);
                    
                }
            }

            double sum = cardsoc + cardtoc;
            if (min != 0)
                dist = 2 * min / sum;
            return dist;
        }

        private static void filtrare_log(string log_file, double prag)
        {
            StreamReader logrdr = new StreamReader(log_file, Encoding.UTF8);
            StreamWriter logfiltrat = new StreamWriter(log_file + ".f", false, Encoding.UTF8);
            string line = "";
            while ((line = logrdr.ReadLine()) != null)
            {

                line = line.Trim();
                line = line.Substring(0, line.Length - 1);
                string[] infos = line.Split('|');
                string to_write = infos[0];
                for (int i = 1; i < infos.Length; i++)
                {
                    string[] spl = infos[i].Split('*');
                    if (Double.Parse(spl[1]) > prag)
                    {
                        to_write = to_write + "|" + spl[0] + "*" + spl[1];
                    }
                }
                logfiltrat.WriteLine(to_write);
            }
            logfiltrat.Close();
            logrdr.Close();

        }
        private static void vectori_coocurente(Dictionary<string, int> wind, Hashtable dict, Hashtable cooc, string side, int frequency)
        {

            StreamWriter wrt = new StreamWriter(side + "vectors", false, Encoding.UTF8);
            foreach (string key in wind.Keys)
            {
                string[] wds = key.Split('^');
                string tkey = wds[0];
                int frecv = wind[key];
                if (frecv > frequency)
                {
                    string to_write = key + "|";
                    foreach (string dkey in dict.Keys)
                    {
                        if (cooc.ContainsKey(tkey + " " + dkey))
                        {
                            to_write = to_write + dkey + "*" + cooc[tkey + " " + dkey] + "|";
                        }
                        if (cooc.ContainsKey(dkey + " " + tkey))
                        {
                            to_write = to_write + dkey + "*" + cooc[dkey + " " + tkey] + "|";
                        }
                    }
                    wrt.WriteLine(to_write);
                }
            }
            wrt.Close();
        }
        static void Main(string[] args)
        {
            string multithreading = "";
            int loading = 0;
            int frequency = 0;
            int window = 0;
            int ll = 3;
            string sourceamverblist = "";
            string targetamverblist = "";
            string crossPOS = "";
            string sPOSlist = "";
            string tPOSlist = "";
            string LD = "";
            read_cfg(".\\cooc.cfg.txt", ref multithreading, ref loading, ref frequency, ref window, ref ll, ref sourceamverblist, ref targetamverblist, ref crossPOS, ref sPOSlist, ref tPOSlist, ref LD);
            splitare_documente_msd(".\\source_corpus");
            int scount = 0;
            Hashtable source_cooc = numarare_coocurente(".\\source_corpus", ref scount, window, sPOSlist);
            Dictionary<string, int> freq_lexical_source = new Dictionary<string, int>();
            Dictionary<string, int> swind = extrage_detradus(".\\source_corpus", ref freq_lexical_source, sPOSlist, sourceamverblist);
            Hashtable sdict = load_dictionary(".\\base_lexicon", "source");
            vectori_coocurente(swind, sdict, source_cooc, "s", frequency);
            calcul_log_direct("svectors", swind, sdict, scount, "log_word_source", freq_lexical_source);
            source_cooc.Clear();
            swind.Clear();

            splitare_documente_msd(".\\target_corpus");
            int tcount = 0;
            Hashtable target_cooc = numarare_coocurente(".\\target_corpus", ref tcount, window, tPOSlist);
            Dictionary<string, int> freq_lexical_target = new Dictionary<string, int>();
            Dictionary<string, int> twind = extrage_detradus(".\\target_corpus", ref freq_lexical_target, tPOSlist, targetamverblist);
            Hashtable tdict = load_dictionary(".\\base_lexicon", "target");
            vectori_coocurente(twind, tdict, target_cooc, "t", frequency);
            calcul_log_direct("tvectors", twind, tdict, tcount, "log_word_target", freq_lexical_target);
            filtrare_log("log_word_source", ll);
            filtrare_log("log_word_target", ll);
            target_cooc.Clear();
            twind.Clear();

            calcul_vectori_ulterior(tdict, sdict);
            tdict.Clear();
            sdict.Clear();
            if (crossPOS == "no")
            {
                separare_pos("source_vectors");
                separare_pos("target_vectors");
                if (multithreading == "yes")
                {
                    basic_similarity_multithread("ncsource", "nctarget", "nc", loading, LD);
                    basic_similarity_multithread("npsource", "nptarget", "np", loading, LD);
                    basic_similarity_multithread("vsource", "vtarget", "v", loading, LD);
                    basic_similarity_multithread("asource", "atarget", "a", loading, LD);
                    basic_similarity_multithread("rsource", "rtarget", "r", loading, LD);
                }
                else
                {
                    basic_similarity("ncsource", "nctarget", "nc", LD);
                    basic_similarity("npsource", "nptarget", "np", LD);
                    basic_similarity("vsource", "vtarget", "v", LD);
                    basic_similarity("asource", "atarget", "a", LD);
                    basic_similarity("rsource", "rtarget", "r", LD);
                }
            }
            else
            {
                if (multithreading == "yes")
                    basic_similarity_multithread("source_vectors", "target_vectors", "total", loading, LD);
                else basic_similarity("source_vectors", "target_vectors", "total", LD);

            }

          
        }

       


    }
}
