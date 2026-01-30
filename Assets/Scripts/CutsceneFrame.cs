using UnityEngine;

[System.Serializable]
public class CutsceneFrame
{
    [Header("Image")]
    public Sprite image;

    [Header("Dialogues")]
    [Tooltip("이 이미지에서 순서대로 출력될 대사들")]
    public string[] dialogues;
}