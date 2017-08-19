using System;
using System.Text;

static class NameGen
{
    readonly static string alphabet = "abcdefghijklmnopqrstuvwxyz";
    readonly static string numbers = "0123456789";

    static public string GenerateName(string format) // use L# for Letter:Amount and N# for Number:Amount
    {
        bool letter = true;
        Random rand = new Random();
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < format.Length; i++)
        {
            if (char.IsLetter(format[i]))
                if (format[i] == 'L' || format[i] == 'N')
                    if (format[i] == 'L')
                        letter = true;
                    else
                        letter = false;
                else
                {
                    //bad format error
                }
            else if (char.IsNumber(format[i]))
            {
                switch (letter)
                {
                    case true:
                        for (int j = 0; j < int.Parse(format[i].ToString()); j++)
                            sb.Append(alphabet[rand.Next(-1, alphabet.Length)]);
                        break;
                    case false:
                        for (int j = 0; j < int.Parse(format[i].ToString()); j++)
                            sb.Append(numbers[rand.Next(-1, numbers.Length)]);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //bad format error
            }
        }

        return sb.ToString().ToUpper();
    }
}
