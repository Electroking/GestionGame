using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVillagerInfos : MonoBehaviour
{
    [HideInInspector] public Villager villager = null;

    [SerializeField] Text nameTxt = null, ageTxt = null, exhaustedTxt = null, hasWorkedTxt = null, jobTxt = null;
    GameObject _jobChangePanel;
    Button _btnShowJobSelection;

    private void Awake()
    {
        _jobChangePanel = transform.GetChild(2).gameObject;
        _btnShowJobSelection = GetComponentInChildren<Button>();
        _btnShowJobSelection.interactable = false;
        _jobChangePanel.SetActive(false);

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (villager == null) return;
        // name
        nameTxt.text = villager.name;
        // age
        ageTxt.text = "Age: " + villager.Age;
        // exhausted
        exhaustedTxt.text = "Status: " + (villager.isExhausted ? "Exhausted" : "Rested");
        // has worked today
        hasWorkedTxt.text = "Has" + (villager.hasWorked ? "" : "n't") + " worked today";
        // job
        jobTxt.text = "Job: " + (villager.job != null ? villager.job.ToString() : "Vagrant");
    }

    public void OnJobClick(int job)
    {
        villager.AssignJob(Job.Type.Student);
        Student student = (Student)villager.job;
        student.jobToLearn = (Job.Type)job;
    }

    public void OnFirstSchoolBuilt()
    {
        LockJobChange(false);
    }

    public void LockJobChange(bool locked)
    {
        if (School.list.Count == 0) return;
        _btnShowJobSelection.interactable = !locked;
        if (locked)
        {
            _jobChangePanel.SetActive(false);
        }
    }

    public void AssignVillager(Villager villager)
    {
        if (this.villager != null)
        {
            this.villager.OnSelect(false);
        }
        this.villager = villager;
        this.villager.OnSelect(true);
    }

    public void ClosePanel()
    {
        if (villager != null) villager.OnSelect(false);
        _jobChangePanel.SetActive(false);
        gameObject.SetActive(false);
    }
}
