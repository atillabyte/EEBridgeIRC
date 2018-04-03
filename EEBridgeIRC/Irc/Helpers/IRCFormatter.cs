using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string FormatIRC(this string input)
    {
        return Regex.Replace(input, @"\[([^\]]*)\]\((.*?)\s*?\s*\)", (match) => {
            var codes_portion = (match.Groups[1].Value).Split(' ', ',');

            var color_check = new Func<string, ColorCode>((value) => Enum.TryParse(value, true, out ColorCode result) ? result : ColorCode.Invalid);
            var control_check = new Func<string, ControlCode>((value) => Enum.TryParse(value, true, out ControlCode result) ? result : ControlCode.Invalid);

            var color_codes = codes_portion.Select(value => color_check.Invoke(value)).Where(value => value != ColorCode.Invalid).ToList();
            var control_codes = codes_portion.Select(value => control_check.Invoke(value)).Where(value => value != ControlCode.Invalid).ToList();

            var color_foreground = color_codes.Count > 0 ? color_codes[0] : ColorCode.Black;
            var color_background = color_codes.Count > 1 ? color_codes[1] : ColorCode.Invalid;

            var color = string.Format("{0}{1}", (int)color_foreground, (color_background != ColorCode.Invalid) ? "," + (int)color_background : "");

            return string.Concat(control_codes.Select(code => (char)code)) + $"\x03{color}" + match.Groups[2].Value + (char)ControlCode.Reset;
        }, RegexOptions.Singleline);
    }
}

public enum ColorCode
{
    Invalid = -1,

    White = 0,
    Black = 1,
    DarkBlue = 2,
    DarkGreen = 3,
    Red = 4,
    DarkRed = 5,
    DarkViolet = 6,
    Orange = 7,
    Yellow = 8,
    LightGreen = 9,
    Cyan = 10,
    LightCyan = 11,
    Blue = 12,
    Violet = 13,
    DarkGray = 14,
    LightGray = 15,

    /* aliases */
    LightGrey = LightGray,
    DarkGrey = DarkGray
};

public enum ControlCode
{
    Invalid = -1,

    Bold = 0x02,
    Color = 0x03,
    Italic = 0x09,
    Reset = 0x0f,
    StrikeThrough = 0x13,
    Underline1 = 0x15,
    Reverse = 0x16,
    Underline2 = 0x1f,

    /* aliases */
    UnderLine = Underline2,
    Italics = Italic,
    Strike = StrikeThrough
};