using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoggedEyeImage {

    //Location to be written to
    string path = "VidLogs\\";

    //Info to be written on either the image name or in the image
    int indexNum = 0;    

    //The image that was captured by camera and convert to unity displayable format
    Texture2D image, fontTexture;
    Font fontStyle;

    public void AddTextToImageTexture(List<string> formattedText) {
        string charsWanted = "`1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";
        FontStyle.RequestCharactersInTexture(charsWanted, 16);
        Texture2D writting = new Texture2D(Image.width, Image.height, TextureFormat.ARGB32, false, true);
        FloodFillArea(writting, Image.width, Image.height, new Color(0, 0, 0, 0));

        WriteMultipleLines(writting, formattedText, 10, 20, 16);
        writting = FlipVertically(writting);

        //Use this to resize the first image, comment it out if not wanted
        //Does havea performance drop
        ResizeImage();
        //writting.Resize(Image.width, Image.height);
        
        //Image = writting;
        image = CombineTextures(Image, writting, 0 , Image.height - writting.height);
        //TextToTexture ttt = new TextToTexture(FontStyle, 10, 10, null, false);
        //Image = ttt.CreateTextToTexture(formattedText, 10, 10, 2, 1, Image);
        //Image = ttt.CreateTextToTexture(formattedText, 10, 10, 150, 2, 1);
    }

    void ResizeImage() {
        TextureScale.Point(Image, (int)(Image.width * 3f), (int)(Image.height * 3f));
        Image.Apply();
    }

    void WriteLine(Texture2D tex, string toWrite, int x, int y) {
        DrawText(tex, toWrite, x, y, FontStyle, FontTexture, 16);
    }

    void WriteMultipleLines(Texture2D tex, List<string> toWrite, int x, int y, int lineSpacing) {
        for(int i = 0; i < toWrite.Count; i++) {
            WriteLine(tex, toWrite[i], x, y + (i * lineSpacing));
        }
        
    }

    Texture2D CombineTextures(Texture2D background, Texture2D watermark, int startX, int startY) {
        Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
        for (int x = 0; x < background.width; x++) {
            for (int y = 0; y < background.height; y++) {
                if (x >= startX && y >= startY && x < watermark.width + startX && y < watermark.height + startY) {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                    newTex.SetPixel(x, y, final_color);
                }
                else
                    newTex.SetPixel(x, y, background.GetPixel(x, y));
            }
        }

        newTex.Apply();
        return newTex;

        //int startX = 0;
        //int startY = firstTex.height - secondTex.height;

        //for (int x = startX; x < firstTex.width; x++) {

        //    for (int y = startY; y < firstTex.height; y++) {
        //        Color bgColor = firstTex.GetPixel(x, y);
        //        Color wmColor = secondTex.GetPixel(x - startX, y - startY);

        //        Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

        //        firstTex.SetPixel(x, y, final_color);
        //    }
        //}

        //firstTex.Apply();
        //return firstTex;

        //if (firstTex.width != secondTex.width || firstTex.height != secondTex.height)
        //    throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
        //var bData = firstTex.GetPixels();
        //var tData = secondTex.GetPixels();
        //int count = bData.Length;
        //var rData = new Color[count];
        //for (int i = 0; i < count; i++) {
        //    Color B = bData[i];
        //    Color T = tData[i];
        //    float srcF = T.a;
        //    float destF = 1f - T.a;
        //    float alpha = srcF + destF * B.a;
        //    Color R = (T * srcF + B * B.a * destF) / alpha;
        //    R.a = alpha;
        //    rData[i] = R;
        //}
        //var res = new Texture2D(secondTex.width, secondTex.height);
        //res.SetPixels(rData);
        //res.Apply();
        //return res;
    }

    public void PrintImageToPng() {
        byte[] bytes = Image.EncodeToPNG();
        File.WriteAllBytes(path + formatLegalPath("EyeData" + indexNum + ".png"), bytes);
    }

    public string formatLegalPath(string e) {
        e = e.Replace(" ", "_");
        e = e.Replace(":", "-");
        e = e.Replace("\\", "-");
        e = e.Replace("/", "-");
        return e;
    }

    public Texture2D Image
    {
        get
        {
            return image;
        }

        set
        {
            image = value;
        }
    }

    public Font FontStyle
    {
        get
        {
            return fontStyle;
        }

        set
        {
            fontStyle = value;
        }
    }

    public Texture2D FontTexture
    {
        get
        {
            return fontTexture;
        }

        set
        {
            fontTexture = value;
        }
    }

    public static void FloodFillArea(Texture2D tex, int nSizeX, int nSizeY, Color col) {
        for (int y = 0; y < nSizeY; y++) {
            for (int x = 0; x < nSizeX; x++) {
                tex.SetPixel(x, y, col);
            }
        }
    }

    private Color GetColor(Color[] pix, int x, int y, int w, int h) {
        return pix[y * w + x];
    }

    private void SetColor(ref Color[] pix, int x, int y, int w, int h, Color c) {
        pix[y * w + x] = c;
    }

    public Color[] FlipRight(Color[] pix, int width, int height) {
        Color cX;

        Color[] a = new Color[pix.Length];

        for (int y = 0; y < height; y++) {

            for (int x = 0; x < width; x++) {
                cX = GetColor(pix, x, y, width, height);

                SetColor(ref a, y, width - 1 - x, height, width, cX);
            }

        }

        return a;
    }

    private void Composite(ref Color[] baseImage, Color[] compositeImage, int w, int h) {
        // composite image with alpha channel onto base image

        for (int y = 0; y < h; ++y) {
            for (int x = 0; x < w; ++x) {
                Color compositeColor = GetColor(compositeImage, x, y, w, h);
                Color baseColor = GetColor(baseImage, x, y, w, h);
                float alpha = compositeColor.a;
                //				float newRed = (1-alpha) * baseColor.r + alpha * compositeColor.r;
                //				float newGreen = (1-alpha) * baseColor.g + alpha * compositeColor.g;
                //				float newBlue = (1-alpha) * baseColor.b + alpha * compositeColor.b;
                //				float newAlpha = 1.0f;
                //				Color newColor = new Color(newRed,newGreen,newBlue,newAlpha);
                Color newColor = Color.Lerp(baseColor, compositeColor, alpha);
                SetColor(ref baseImage, x, y, w, h, newColor);
            }
        }
    }

    

    public Texture2D DrawText(Texture2D tx, string sText, int offsetX, int offsetY, Font font1, Texture2D font1Tx, int fontSize) {
        CharacterInfo ci;
        char[] cText = sText.ToCharArray();

        //Material fontMat = font1.material;
        //Texture2D fontTx = (Texture2D) fontMat.mainTexture;
        Texture2D fontTx = font1Tx;

        int x, y, w, h;
        int posX = 0;

        font1.GetCharacterInfo('I', out ci, fontSize);

        //int IHeight = (int) (ci.flipped ? (float) fontTx.width * (ci.uv.width) : (float) fontTx.height * (-ci.uv.height));

        for (int i = 0; i < cText.Length; i++) {
            font1.GetCharacterInfo(cText[i], out ci, fontSize);

            x = (int)((float)fontTx.width * ci.uv.x);
            y = (int)((float)fontTx.height * (ci.uv.y + ci.uv.height));
            w = (int)((float)fontTx.width * ci.uv.width);
            h = (int)((float)fontTx.height * (-ci.uv.height));

            //Debug.Log("Stats: " + x + " " + y + " " + w + " " + h);

            Color[] charPix = fontTx.GetPixels(x, y, w, h);

            if (ci.flipped) {
                charPix = FlipRight(charPix, w, h);

                x = posX;
                //y = (IHeight - w) + w/2;
                y = (int)-ci.vert.y;

                int tmp = w;
                w = h;
                h = tmp;
            }
            else {
                x = posX + (int)ci.vert.x;
                y = (int)-ci.vert.y;
            }

            Color[] basePix = tx.GetPixels(offsetX + x, offsetY + y, w, h);

            Composite(ref basePix, charPix, w, h);

            tx.SetPixels(offsetX + x, offsetY + y, w, h, basePix);

            posX += (int)ci.width;
        }
        return tx;
    }

    public Color[] FlipVertically(Color[] pix, int width, int height) {
        int row, targetRow, targetRowStart;
        Color[] a = new Color[pix.Length];

        for (int i = 0; i < pix.Length;) {
            row = i / width;
            targetRow = height - row;
            targetRowStart = (targetRow - 1) * width;

            for (int j = targetRowStart; j < targetRowStart + width; j++, i++) {
                a[j] = pix[i];
            }
        }

        return a;
    }

    public Texture2D FlipVertically(Texture2D tex) {
        tex.SetPixels(FlipVertically(tex.GetPixels(), tex.width, tex.height));
        return tex;
    }
}

public class TextToTexture {
    private const int ASCII_START_OFFSET = 32;
    private Font customFont;
    private Texture2D fontTexture;
    private int fontCountX;
    private int fontCountY;
    private float[] kerningValues;
    private bool supportSpecialCharacters;

    public TextToTexture(Font customFont, int fontCountX, int fontCountY, PerCharacterKerning[] perCharacterKerning, bool supportSpecialCharacters) {
        this.customFont = customFont;
        fontTexture = (Texture2D)customFont.material.mainTexture;
        this.fontCountX = fontCountX;
        this.fontCountY = fontCountY;
        if(perCharacterKerning != null) {
            kerningValues = GetCharacterKerningValuesFromPerCharacterKerning(perCharacterKerning);
        }
        
        this.supportSpecialCharacters = supportSpecialCharacters;
    }

    //placementX and Y - placement within texture size, texture size = textureWidth and textureHeight (square)
    public Texture2D CreateTextToTexture(string text, int textPlacementX, int textPlacementY, int textureSize, float characterSize, float lineSpacing) {
        Texture2D txtTexture = CreatefillTexture2D(Color.clear, textureSize, textureSize);
        int fontGridCellWidth = (int)(fontTexture.width / fontCountX);
        int fontGridCellHeight = (int)(fontTexture.height / fontCountY);
        int fontItemWidth = (int)(fontGridCellWidth * characterSize);
        int fontItemHeight = (int)(fontGridCellHeight * characterSize);
        Vector2 charTexturePos;
        Color[] charPixels;
        float textPosX = textPlacementX;
        float textPosY = textPlacementY;
        float charKerning;
        bool nextCharacterSpecial = false;
        char letter;

        for (int n = 0; n < text.Length; n++) {
            letter = text[n];
            nextCharacterSpecial = false;

            if (letter == '\\' && supportSpecialCharacters) {
                nextCharacterSpecial = true;
                if (n + 1 < text.Length) {
                    n++;
                    letter = text[n];
                    if (letter == 'n' || letter == 'r') //new line or return
                    {
                        textPosY -= fontItemHeight * lineSpacing;
                        textPosX = textPlacementX;
                    }
                    else if (letter == 't') {
                        textPosX += fontItemWidth * GetKerningValue(' ') * 5; //5 spaces
                    }
                    else if (letter == '\\') {
                        nextCharacterSpecial = false; //this allows for printing of \
                    }
                }
            }

            if (!nextCharacterSpecial && customFont.HasCharacter(letter)) {
                charTexturePos = GetCharacterGridPosition(letter);
                charTexturePos.x *= fontGridCellWidth;
                charTexturePos.y *= fontGridCellHeight;
                charPixels = fontTexture.GetPixels((int)charTexturePos.x, fontTexture.height - (int)charTexturePos.y - fontGridCellHeight, fontGridCellWidth, fontGridCellHeight);
                charPixels = changeDimensions(charPixels, fontGridCellWidth, fontGridCellHeight, fontItemWidth, fontItemHeight);

                txtTexture = AddPixelsToTextureIfClear(txtTexture, charPixels, (int)textPosX, (int)textPosY, fontItemWidth, fontItemHeight);
                //charKerning = GetKerningValue(letter);
                textPosX += (fontItemWidth * 0.201f); //add kerning here
            }
            else if (!nextCharacterSpecial) {
                Debug.Log("Letter Not Found:" + letter);
            }

        }
        txtTexture.Apply();
        return txtTexture;
    }

    public Texture2D CreateTextToTexture(string text, int textPlacementX, int textPlacementY, float characterSize, float lineSpacing, Texture2D tex) {
        //Texture2D txtTexture = CreatefillTexture2D(Color.clear, textureSize, textureSize);
        int fontGridCellWidth = (int)(fontTexture.width / fontCountX);
        int fontGridCellHeight = (int)(fontTexture.height / fontCountY);
        int fontItemWidth = (int)(fontGridCellWidth * characterSize);
        int fontItemHeight = (int)(fontGridCellHeight * characterSize);
        Vector2 charTexturePos;
        Color[] charPixels;
        float textPosX = textPlacementX;
        float textPosY = textPlacementY;
        float charKerning;
        bool nextCharacterSpecial = false;
        char letter;

        for (int n = 0; n < text.Length; n++) {
            letter = text[n];
            nextCharacterSpecial = false;

            if (letter == '\\' && supportSpecialCharacters) {
                nextCharacterSpecial = true;
                if (n + 1 < text.Length) {
                    n++;
                    letter = text[n];
                    if (letter == 'n' || letter == 'r') //new line or return
                    {
                        textPosY -= fontItemHeight * lineSpacing;
                        textPosX = textPlacementX;
                    }
                    else if (letter == 't') {
                        textPosX += fontItemWidth * GetKerningValue(' ') * 5; //5 spaces
                    }
                    else if (letter == '\\') {
                        nextCharacterSpecial = false; //this allows for printing of \
                    }
                }
            }

            if (!nextCharacterSpecial && customFont.HasCharacter(letter)) {
                charTexturePos = GetCharacterGridPosition(letter);
                charTexturePos.x *= fontGridCellWidth;
                charTexturePos.y *= fontGridCellHeight;
                Debug.Log("x: " + (int)charTexturePos.x + " y: " + (fontTexture.height - (int)charTexturePos.y - fontGridCellHeight) + " width: " + fontGridCellWidth + " height:" + fontGridCellHeight);
                charPixels = fontTexture.GetPixels((int)charTexturePos.x, fontTexture.height - (int)charTexturePos.y - fontGridCellHeight, fontGridCellWidth, fontGridCellHeight);
                charPixels = changeDimensions(charPixels, fontGridCellWidth, fontGridCellHeight, fontItemWidth, fontItemHeight);

                tex = AddPixelsToTextureIfClear(tex, charPixels, (int)textPosX, (int)textPosY, fontItemWidth, fontItemHeight);
                //charKerning = GetKerningValue(letter);
                textPosX += (fontItemWidth * 0.201f); //add kerning here
            }
            else if (!nextCharacterSpecial) {
                Debug.Log("Letter Not Found:" + letter);
            }

        }
        tex.Apply();
        return tex;
    }

    //doesn't yet support special characters
    //trailing buffer is to allow for area where the character might be at the end
    public int CalcTextWidthPlusTrailingBuffer(string text, int decalTextureSize, float characterSize) {
        char letter;
        float width = 0;
        int fontItemWidth = (int)((fontTexture.width / fontCountX) * characterSize);

        for (int n = 0; n < text.Length; n++) {
            letter = text[n];
            if (n < text.Length - 1) {
                width += fontItemWidth * GetKerningValue(letter);
            }
            else //last letter ignore kerning for buffer
            {
                width += fontItemWidth;
            }
        }

        return (int)width;
    }

    //look for a faster way of calculating this
    private Color[] changeDimensions(Color[] originalColors, int originalWidth, int originalHeight, int newWidth, int newHeight) {
        Color[] newColors;
        Texture2D originalTexture;
        int pixelCount;
        float u;
        float v;

        if (originalWidth == newWidth && originalHeight == newHeight) {
            newColors = originalColors;
        }
        else {
            newColors = new Color[newWidth * newHeight];
            originalTexture = new Texture2D(originalWidth, originalHeight);

            originalTexture.SetPixels(originalColors);
            for (int y = 0; y < newHeight; y++) {
                for (int x = 0; x < newWidth; x++) {
                    pixelCount = x + (y * newWidth);
                    u = (float)x / newWidth;
                    v = (float)y / newHeight;
                    newColors[pixelCount] = originalTexture.GetPixelBilinear(u, v);
                }
            }
        }

        return newColors;
    }

    //add pixels to texture if pixels are currently clear
    private Texture2D AddPixelsToTextureIfClear(Texture2D texture, Color[] newPixels, int placementX, int placementY, int placementWidth, int placementHeight) {
        int pixelCount = 0;
        Color[] currPixels;

        if (placementX + placementWidth < texture.width) {
            currPixels = texture.GetPixels(placementX, placementY, placementWidth, placementHeight);

            for (int y = 0; y < placementHeight; y++) //vert
            {
                for (int x = 0; x < placementWidth; x++) //horiz
                {
                    pixelCount = x + (y * placementWidth);
                    if (currPixels[pixelCount] != Color.clear) //if pixel is not empty take the previous value
                    {
                        newPixels[pixelCount] = currPixels[pixelCount];
                    }
                }
            }

            texture.SetPixels(placementX, placementY, placementWidth, placementHeight, newPixels);
        }
        else {
            Debug.Log("Letter Falls Outside Bounds of Texture" + (placementX + placementWidth));
        }
        return texture;
    }

    private Vector2 GetCharacterGridPosition(char c) {
        int codeOffset = c - ASCII_START_OFFSET;

        return new Vector2(codeOffset % fontCountX, (int)codeOffset / fontCountX);
    }

    private float GetKerningValue(char c) {
        return kerningValues[((int)c) - ASCII_START_OFFSET];
    }

    private Texture2D CreatefillTexture2D(Color color, int textureWidth, int textureHeight) {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        int numOfPixels = texture.width * texture.height;
        Color[] colors = new Color[numOfPixels];
        for (int x = 0; x < numOfPixels; x++) {
            colors[x] = color;
        }

        texture.SetPixels(colors);

        return texture;
    }

    private float[] GetCharacterKerningValuesFromPerCharacterKerning(PerCharacterKerning[] perCharacterKerning) {
        float[] perCharKerning = new float[128 - ASCII_START_OFFSET];
        int charCode;

        foreach (PerCharacterKerning perCharKerningObj in perCharacterKerning) {
            if (perCharKerningObj.First != "") {
                charCode = (int)perCharKerningObj.GetChar(); ;
                if (charCode >= 0 && charCode - ASCII_START_OFFSET < perCharKerning.Length) {
                    perCharKerning[charCode - ASCII_START_OFFSET] = perCharKerningObj.GetKerningValue();
                }
            }
        }
        return perCharKerning;
    }
}

[System.Serializable]
public class PerCharacterKerning {
    //these are named First and Second not because I'm terrible at naming things, but to make it look and act more like Custom Font natively do within Unity
    public string First = ""; //character
    public float Second; //kerning ex: 0.201

    public PerCharacterKerning(string character, float kerning) {
        this.First = character;
        this.Second = kerning;
    }

    public PerCharacterKerning(char character, float kerning) {
        this.First = "" + character;
        this.Second = kerning;
    }

    public char GetChar() {
        return First[0];
    }
    public float GetKerningValue() { return Second; }

    public static float[] DefaultCharacterKerning() {
        double[] perCharKerningDouble = new double[] {
 .201 /* */
,.201 /*!*/
,.256 /*"*/
,.401 /*#*/
,.401 /*$*/
,.641 /*%*/
,.481 /*&*/
,.138 /*'*/
,.24 /*(*/
,.24 /*)*/
,.281 /***/
,.421 /*+*/
,.201 /*,*/
,.24 /*-*/
,.201 /*.*/
,.201 /*/*/
,.401 /*0*/
,.353 /*1*/
,.401 /*2*/
,.401 /*3*/
,.401 /*4*/
,.401 /*5*/
,.401 /*6*/
,.401 /*7*/
,.401 /*8*/
,.401 /*9*/
,.201 /*:*/
,.201 /*;*/
,.421 /*<*/
,.421 /*=*/
,.421 /*>*/
,.401 /*?*/
,.731 /*@*/
,.481 /*A*/
,.481 /*B*/
,.52  /*C*/
,.481 /*D*/
,.481 /*E*/
,.44  /*F*/
,.561 /*G*/
,.52  /*H*/
,.201 /*I*/
,.36  /*J*/
,.481 /*K*/
,.401 /*L*/
,.6   /*M*/
,.52  /*N*/
,.561 /*O*/
,.481 /*P*/
,.561 /*Q*/
,.52  /*R*/
,.481 /*S*/
,.44  /*T*/
,.52  /*U*/
,.481 /*V*/
,.68  /*W*/
,.481 /*X*/
,.481 /*Y*/
,.44  /*Z*/
,.201 /*[*/
,.201 /*\*/
,.201 /*]*/
,.338 /*^*/
,.401 /*_*/
,.24  /*`*/
,.401 /*a*/
,.401 /*b*/
,.36  /*c*/
,.401 /*d*/
,.401 /*e*/
,.189 /*f*/
,.401 /*g*/
,.401 /*h*/
,.16  /*i*/
,.16  /*j*/
,.36  /*k*/
,.16  /*l*/
,.6   /*m*/
,.401 /*n*/
,.401 /*o*/
,.401 /*p*/
,.401 /*q*/
,.24  /*r*/
,.36  /*s*/
,.201 /*t*/
,.401 /*u*/
,.36  /*v*/
,.52  /*w*/
,.36  /*x*/
,.36  /*y*/
,.36  /*z*/
,.241 /*{*/
,.188 /*|*/
,.241 /*}*/
,.421 /*~*/
};
        float[] perCharKerning = new float[perCharKerningDouble.Length];

        for (int x = 0; x < perCharKerning.Length; x++) {
            perCharKerning[x] = (float)perCharKerningDouble[x];
        }
        return perCharKerning;
    }
}
