using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    TITLE,
    MENU,
    STORYMODE,
    CUSTOMGAME,
    OPTION,
    FIRSTSTORY,
    SECONDSTORY = FIRSTSTORY + 1,
    THIRDSTORY = FIRSTSTORY + 2,
    OPTION_KEY,
    OPTION_SOUND
};