using UnityEngine;

public static class EditorTextures
{
    private static string IconsPath = "Icons";
    private static string TexturesPath = "Textures";
    private static Texture communityTexture;
    private static Texture githubSponsorsTexture;
    private static Texture githubTexture;
    private static Texture homePageTexture;
    private static Texture githubIcon;
    private static Texture fantasyCommunityIcon;
    private static Texture unityCommunity_Icon;
    private static Texture taikrCommunity_Icon;
    private static Texture magicBoxCommunity_Icon;
    private static Texture kerryTaCommunity_Icon;

    public static Texture KerryTaCommunity_Icon
    {
        get {
            if (kerryTaCommunity_Icon == null)
                kerryTaCommunity_Icon = Resources.Load<Texture>($"{IconsPath}\\KerryTaCommunity_Icon");
            return kerryTaCommunity_Icon;
        }
    }

    public static Texture MagicBoxCommunity_Icon
    {
        get {
            if (magicBoxCommunity_Icon == null)
                magicBoxCommunity_Icon = Resources.Load<Texture>($"{IconsPath}\\MagicBoxCommunity_Icon");
            return magicBoxCommunity_Icon;
        }
    }

    public static Texture TaikrCommunity_Icon
    {
        get {
            if (taikrCommunity_Icon == null)
                taikrCommunity_Icon = Resources.Load<Texture>($"{IconsPath}\\TaikrCommunity_Icon");
            return taikrCommunity_Icon;
        }
    }


    public static Texture UnityCommunity_Icon
    {
        get {
            if (unityCommunity_Icon == null)
                unityCommunity_Icon = Resources.Load<Texture>($"{IconsPath}\\UnityCommunity_Icon");
            return unityCommunity_Icon;
        }
    }

    public static Texture FantasyCommunityIcon
    {
        get {
            if (fantasyCommunityIcon == null)
                fantasyCommunityIcon = Resources.Load<Texture>($"{IconsPath}\\FantasyCommunity_Icon");
            return fantasyCommunityIcon;
        }
    }

    public static Texture GithubIcon
    {
        get {
            if (githubIcon == null)
                githubIcon = Resources.Load<Texture>($"{IconsPath}\\Github_Icon");
            return githubIcon;
        }
    }

    public static Texture HomePageTexture
    {
        get {
            if (homePageTexture == null)
                homePageTexture = Resources.Load<Texture>($"{IconsPath}\\HomePage_icon");
            return homePageTexture;
        }
    }

    public static Texture CommunityTexture
    {
        get {
            if (communityTexture == null)
                communityTexture = Resources.Load<Texture>($"{TexturesPath}\\CommunityTexture");
            return communityTexture;
        }
    }

    public static Texture GithubSponsorsTexture
    {
        get {
            if (githubSponsorsTexture == null)
                githubSponsorsTexture = Resources.Load<Texture>($"{TexturesPath}\\GithubSponsorsTexture");
            return githubSponsorsTexture;
        }
    }

    public static Texture GithubTexture
    {
        get {
            if (githubTexture == null)
                githubTexture = Resources.Load<Texture>($"{TexturesPath}\\GithubTexture");
            return githubTexture;
        }
    }
}