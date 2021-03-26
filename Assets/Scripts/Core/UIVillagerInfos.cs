using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVillagerInfos : MonoBehaviour
{
    public Villager villager;
    [SerializeField] Text villagerNameTxt;
    [SerializeField] GameObject jobChangePanel;

    private void Update()
    {
        villagerNameTxt.text = villager.name;
    }

    public void OnJobClick(int job)
    {
        villager.AssignJob(Job.Type.Student);
        Student student = (Student)villager.job;
        student.jobToLearn = (Job.Type)job;
    }
}
