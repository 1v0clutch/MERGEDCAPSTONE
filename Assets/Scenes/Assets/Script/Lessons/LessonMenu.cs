using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LessonMenu : MonoBehaviour
{
    [SerializeField] GameObject lessonMenu;
    public void AccessLesson()
    {
        lessonMenu.SetActive(true);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    public void CloseLesson()
    {
        lessonMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
