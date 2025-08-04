
public static class GameConfig
{
    // path resources
    public const string PATH_UI = "UI/"; // duong dan toi cac canvas UI trong Resources

    // tile anim
    public const string ANIM_TILE_APPEAR = "appear"; // ten cua trigger khi tile xuat hien
    public const string ANIM_TILE_DISAPPEAR = "disappear"; // ten cua trigger khi tile bien mat
    public const string ANIM_TILE_SELECT = "selecting"; // ten cua trigger khi tile duoc chon
    public const string ANIM_TILE_UNSELECT = "unselecting"; // ten cua trigger khi tile khong duoc chon
    public const string ANIM_TILE_MATCH = "match"; // ten cua trigger khi tile duoc match

    // menu anim
    public const string ANIM_MENU_BUTTON_EXPAND = "isExpanded"; // ten cua trigger khi menu button duoc mo rong
    public const string ANIM_MENU_BUTTON_COLLAPSE = "isCollapsed"; // ten cua trigger khi menu button duoc thu gon

    // anim popup
    public const string ANIM_SETTING_OPEN = "open"; // ten cua trigger khi popup mo
    public const string ANIM_SETTING_CLOSE = "exit"; // ten cua trigger khi popup dong

    // anim xuat hien, ket thuc man hien tai
    public const string ANIM_TRANSITION_OPEN = "open"; // ten cua trigger khi open canvas
    public const string ANIM_TRANSITION_CLOSE = "close"; // ten cua trigger khi close canvas

    // path json to save data
    public const string NAME_FOLDER_SAVE_USERS = "SavedUsers"; // ten thu muc luu user data
    public const string NAME_FOLDER_SAVE_LEVELS = "SavedLevels"; // ten thu muc luu level data

    // playerpref sound
    public const string SOUND_KEY = "sound";
    public const string FX_KEY = "fx";
}
