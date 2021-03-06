﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_excel_lab2
{
    public class Indexator
    {
        const int countOfLetters = 26;
        const int indexAinASCII = 65;

        public int pow(int x, int n)
        {
            if (n == 0 && x!= 0) return 1;
            for(int i=0; i<n-1; i++)
            {
                x *= x;
            }
            return x;
        }
         
        public string ChangeLetter(string s, char c, int pos)
        {
            string res = "";
            for(int i=0; i<pos; i++)
            {
                res += s[i];
            }
            res += c;
            if(pos<=s.Length-1)
            {
                for(int i = pos+1; i<s.Length; i++)
                {
                    res += s[i];
                }
            }
            return res;
        }

        private string nextWordHelp(string s, int zero)
        {
            try
            {
                int pos = s.Length - zero - 1;
                if (s[pos] == 'Z')
                {
                    s = ChangeLetter(s, 'A', pos);
                    if (pos == 0) return s + 'A';
                    else return nextWordHelp(s, zero + 1);
                }
                else s = ChangeLetter(s, (char)(s[pos] + 1), pos);

                return s;
            }
            catch (System.IndexOutOfRangeException)
            { return "A"; }
          
        }

        public string NextWord(string s)
        {
            return nextWordHelp(s, 0);
        }


        public int fromWordToNumber(string s)
        {
            int res = 0;
            int n = s.Length;
            for(int i=0; i<n; i++)
            {
                res += pow(26, n - i - 1) * ((s[i] - 0) - indexAinASCII +1);
            }
            return res;
        }

        public string FromNumberToWord(int n)
        {
            string res = "A";
            while(fromWordToNumber(res) != n)
            {
                res = NextWord(res);
            }
            return res;
        }
    }
}
