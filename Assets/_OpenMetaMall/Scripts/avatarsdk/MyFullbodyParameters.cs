using ItSeez3D.AvatarSdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ItSeez3D.AvatarSdkSamples.Core
{
    public class MyFullbodyParameters : MyGettingStarted
    {
        public FullbodyParametersConfigurationPanel parametersPanel = null;

        public GameObject baseControlsParent;

        public GameObject generatedAvatarControlsParent;

        public GameObject haircutsControlsParent;

        public GameObject outfitsControlsParent;

        public GameObject facialAnimationsControlsParent;

        public GameObject bodyAnimationsControlsParent;

        public Text haircutNameText;

        public Text outfitNameText;

        public Text facialAnimationNameText;

        public Text bodyAnimationNameText;

        public ItemsSelectingView haircutsSelectingView;

        public ItemsSelectingView outfitsSelectingView;

        public ModelInfoDataPanel modelInfoPanel;

        public RuntimeAnimatorController facialAnimatorController;

        public FullbodyAnimatorsHolder fullbodyAnimatorsHolder;

        private IFullbodyAvatarProvider fullbodyAvatarProvider = null;

        private bool isParametersPanelActive = false;

        private bool isAvatarDisplayed = false;

        private FullbodyAvatarLoader avatarLoader = null;

        private int currentHaircutIdx = -1;
        private List<string> haircuts = new List<string>();

        private int currentOutfitIdx = -1;
        private List<string> outfits = new List<string>();

        AnimationManager facialAnimationManager = null;
        FullbodyAnimationManager bodyAnimationManager = null;

        private readonly string generatedHaircutName = "generated";
        private readonly string baldHaircutName = "bald";
        private readonly string emptyOutfitName = "no outfit";

        private GameObject avatarHolder;
        [SerializeField] string sceneToLoad = "Environment";

        [SerializeField] GameObject customPlayer;

        [SerializeField] string avatarCode;
        public string AvatarCode { get { return avatarCode; } set { avatarCode = value; } }

        #region public methods
        public MyFullbodyParameters()
        {
            selectedPipelineType = PipelineType.FIT_PERSON;
        }

        public void OnParametersButtonClick()
        {
            SwitchParametersPanelState();
        }

        public void OnNextHaircutButtonClick()
        {
            currentHaircutIdx++;
            if (currentHaircutIdx >= haircuts.Count)
                currentHaircutIdx = 0;
            StartCoroutine(ChangeHaircut(haircuts[currentHaircutIdx]));
        }

        public void OnPrevHaircutButtonClick()
        {
            currentHaircutIdx--;
            if (currentHaircutIdx < 0)
                currentHaircutIdx = haircuts.Count - 1;
            StartCoroutine(ChangeHaircut(haircuts[currentHaircutIdx]));
        }

        public void OnHaircutListButtonClick()
        {
            baseControlsParent.SetActive(false);
            string currentHaircutName = haircuts[currentHaircutIdx];
            haircutsSelectingView.Show(new List<string>() { currentHaircutName }, (list, isSelected) =>
            {
                baseControlsParent.SetActive(true);
                if (isSelected)
                    StartCoroutine(ChangeHaircut(list[0]));
            });
        }

        public void OnNextOutfitButtonClick()
        {
            currentOutfitIdx++;
            if (currentOutfitIdx >= outfits.Count)
                currentOutfitIdx = 0;
            StartCoroutine(ChangeOutfit(outfits[currentOutfitIdx]));
        }

        public void OnPrevOutfitButtonClick()
        {
            currentOutfitIdx--;
            if (currentOutfitIdx < 0)
                currentOutfitIdx = outfits.Count - 1;
            StartCoroutine(ChangeOutfit(outfits[currentOutfitIdx]));
        }

        public void OnOutfitListButtonClick()
        {
            baseControlsParent.SetActive(false);
            string currentOutfitName = outfits[currentOutfitIdx];
            outfitsSelectingView.Show(new List<string>() { currentOutfitName }, (list, isSelected) =>
            {
                baseControlsParent.SetActive(true);
                if (isSelected)
                    StartCoroutine(ChangeOutfit(list[0]));
            });
        }

        public void OnNextFacialAnimationClick()
        {
            if (facialAnimationManager != null)
                facialAnimationManager.PlayNextAnimation();
        }

        public void OnPrevFacialAnimationClick()
        {
            if (facialAnimationManager != null)
                facialAnimationManager.PlayPrevAnimation();
        }

        public void OnCurrentFacialAnimationClick()
        {
            if (facialAnimationManager != null)
                facialAnimationManager.PlayCurrentAnimation();
        }

        public void OnNextBodyAnimationClick()
        {
            if (bodyAnimationManager != null)
                bodyAnimationManager.PlayNextAnimation();
        }

        public void OnPrevBodyAnimationClick()
        {
            if (bodyAnimationManager != null)
                bodyAnimationManager.PlayPrevAnimation();
        }

        public void OnCurrentBodyAnimationClick()
        {
            if (bodyAnimationManager != null)
                bodyAnimationManager.PlayCurrentAnimation();
        }

        #endregion public methods

        #region base overrided methods
        protected override void Start()
        {
            base.Start();

            if (initParams != null)
            {
                Debug.LogFormat("Init Params:Loading Avatar:" + initParams.avatarCode);
            }

            if ((SceneManager.GetActiveScene().name == "Environment")
            && initParams != null && initParams.isCustomPlayer)
            {
                Debug.Log("activate custom player");
                //customPlayer.SetActive(true);
                //this.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(MyLoadAvatar());
            }
        }

        private IEnumerator MyLoadAvatar()
        {
            yield return StartCoroutine(MyDisplayHead(selectedPipelineType));
        }

        public override void GenerateRandomAvatar()
        {
            byte[] photoBytes = null;
            if (selectedPipelineType == PipelineType.META_PERSON_MALE)
                photoBytes = photoSupplier.GetMalePhoto();
            else if (selectedPipelineType == PipelineType.META_PERSON_FEMALE)
                photoBytes = photoSupplier.GetFemalePhoto();
            else
                photoBytes = photoSupplier.GetRandomPhoto();

            StartCoroutine(GenerateAvatarFunc(photoBytes));
        }

        protected override IEnumerator Initialize()
        {
            fullbodyAvatarProvider = AvatarSdkMgr.GetFullbodyAvatarProvider();
            avatarProvider = fullbodyAvatarProvider;
            yield return Await(avatarProvider.InitializeAsync());
            yield return CheckAvailablePipeline();
        }

        protected IEnumerator MyDisplayHead(PipelineType pipeline)
        {
            isAvatarDisplayed = false;
            generatedAvatarPipeline = pipeline;
            // generatedAvatarControlsParent.SetActive(false);

            // if (isParametersPanelActive)
            // 				OnParametersButtonClick();

            FullbodyAvatarComputationParameters computationParameters = new FullbodyAvatarComputationParameters();
            computationParameters.blendshapes.names.Add("mobile_51");

            if (parametersPanel != null)
            {
                parametersPanel.ConfigureComputationParameters(computationParameters);
            }

            // generate avatar from the photo and get its code in the Result of request
            // var initializeRequest = fullbodyAvatarProvider.InitializeFullbodyAvatarAsync(photoBytes, computationParameters, selectedPipelineType);
            // yield return Await(initializeRequest);
            // currentAvatarCode = initializeRequest.Result;

            if (SceneManager.GetActiveScene().name == "Avatar Viewer")
            {
                currentAvatarCode = initParams != null ? initParams.avatarCode : "d1c2781c-306f-4df7-b1b8-e10af99e396b";

            }
            else if (SceneManager.GetActiveScene().name == "Environment")
            {
                currentAvatarCode = initParams != null ? avatarCode : "d1c2781c-306f-4df7-b1b8-e10af99e396b";

            }


            StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

            //Await till avatar is calculated
            var calculateRequest = fullbodyAvatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
            yield return Await(calculateRequest);

            string initialHaircutName = GetInitialHaircutName(currentAvatarCode);
            string initialOutfitName = GetInitialOutfitName(currentAvatarCode);
            if (AvatarSdkMgr.GetSdkType() == SdkType.Cloud)
            {
                //Downloading avatar model files
                var retrievingBodyModelRequest = fullbodyAvatarProvider.RetrieveBodyModelFromCloudAsync(currentAvatarCode);
                yield return Await(retrievingBodyModelRequest);

                //Downloading haircut if required
                if (!string.IsNullOrEmpty(initialHaircutName) && !computationParameters.haircuts.embed)
                {
                    var retrievingHaircutRequest = fullbodyAvatarProvider.RetrieveHaircutModelFromCloudAsync(currentAvatarCode, initialHaircutName);
                    yield return Await(retrievingHaircutRequest);
                }

                //Downloading outfit if required
                if (!string.IsNullOrEmpty(initialOutfitName) && !computationParameters.outfits.embed)
                {
                    var retrievingOutfitRequest = fullbodyAvatarProvider.RetrieveOutfitModelFromCloudAsync(currentAvatarCode, initialOutfitName);
                    yield return Await(retrievingOutfitRequest);
                }
            }

            yield return ShowAvatarOnScene(initialHaircutName, initialOutfitName, computationParameters);

            if (SceneManager.GetActiveScene().name == "Avatar Viewer")
            {
                ConfigureHaircutsControls(initialHaircutName);
                ConfigureOutfitsControls(initialOutfitName);
            }

            ConfigureFacialAnimationsControls(computationParameters.blendshapes.names);
            ConfigureBodyAnimationsControls(computationParameters.template == ExportTemplate.FULLBODY, pipeline);

            ModelInfo modelInfo = CoreTools.GetAvatarModelInfo(currentAvatarCode);
            // modelInfoPanel.UpdateData(modelInfo);

            progressText.text = string.Empty;

            if (generatedAvatarControlsParent != null)
            {
                generatedAvatarControlsParent.SetActive(true);
            }

            avatarHolder.GetComponent<DynamicPlayerAnimation>().anim = avatarLoader.AvatarGameObject.GetComponent<FullbodyAnimationManager>();

            //avatarHolder.GetComponent<DynamicPlayerAnimation>().animator = avatarLoader.AvatarGameObject.GetComponent<Animator>();

            isAvatarDisplayed = true;
        }

        protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
        {
            isAvatarDisplayed = false;
            generatedAvatarPipeline = pipeline;
            generatedAvatarControlsParent.SetActive(false);

            if (isParametersPanelActive)
                OnParametersButtonClick();

            FullbodyAvatarComputationParameters computationParameters = new FullbodyAvatarComputationParameters();
            parametersPanel.ConfigureComputationParameters(computationParameters);

            // generate avatar from the photo and get its code in the Result of request
            var initializeRequest = fullbodyAvatarProvider.InitializeFullbodyAvatarAsync(photoBytes, computationParameters, selectedPipelineType);
            yield return Await(initializeRequest);
            currentAvatarCode = initializeRequest.Result;

            StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

            //Await till avatar is calculated
            var calculateRequest = fullbodyAvatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
            yield return Await(calculateRequest);

            string initialHaircutName = GetInitialHaircutName(currentAvatarCode);
            string initialOutfitName = GetInitialOutfitName(currentAvatarCode);
            if (AvatarSdkMgr.GetSdkType() == SdkType.Cloud)
            {
                //Downloading avatar model files
                var retrievingBodyModelRequest = fullbodyAvatarProvider.RetrieveBodyModelFromCloudAsync(currentAvatarCode);
                yield return Await(retrievingBodyModelRequest);

                //Downloading haircut if required
                if (!string.IsNullOrEmpty(initialHaircutName) && !computationParameters.haircuts.embed)
                {
                    var retrievingHaircutRequest = fullbodyAvatarProvider.RetrieveHaircutModelFromCloudAsync(currentAvatarCode, initialHaircutName);
                    yield return Await(retrievingHaircutRequest);
                }

                //Downloading outfit if required
                if (!string.IsNullOrEmpty(initialOutfitName) && !computationParameters.outfits.embed)
                {
                    var retrievingOutfitRequest = fullbodyAvatarProvider.RetrieveOutfitModelFromCloudAsync(currentAvatarCode, initialOutfitName);
                    yield return Await(retrievingOutfitRequest);
                }
            }

            yield return ShowAvatarOnScene(initialHaircutName, initialOutfitName, computationParameters);

            ConfigureHaircutsControls(initialHaircutName);
            ConfigureOutfitsControls(initialOutfitName);
            ConfigureFacialAnimationsControls(computationParameters.blendshapes.names);
            ConfigureBodyAnimationsControls(computationParameters.template == ExportTemplate.FULLBODY, pipeline);

            ModelInfo modelInfo = CoreTools.GetAvatarModelInfo(currentAvatarCode);
            modelInfoPanel.UpdateData(modelInfo);

            progressText.text = string.Empty;
            generatedAvatarControlsParent.SetActive(true);
            isAvatarDisplayed = true;
        }

        protected override void OnPipelineTypeToggleChanged(PipelineType pipelineType)
        {
            base.OnPipelineTypeToggleChanged(pipelineType);

            if (avatarProvider != null && avatarProvider.IsInitialized)
                StartCoroutine(UpdateAvatarParameters());
        }

        protected override void SetControlsInteractable(bool interactable)
        {
            base.SetControlsInteractable(interactable);

            /* parametersPanel.SetControlsInteractable(interactable);

			Selectable[] generatedControls = generatedAvatarControlsParent.GetComponentsInChildren<Selectable>(true);
			if (generatedControls != null)
			{
				foreach (Selectable c in generatedControls)
					c.interactable = interactable;
			}*/
        }

        public void OnBackToStartSceneClick()
        {
            avatarProvider.Dispose();
            SceneManager.LoadScene("Avatar Generation");
        }
        public void OnEnterToMetaMallClick()
        {
            avatarProvider.Dispose();
            SceneManager.LoadScene(sceneToLoad);
        }

        #endregion

        #region private methods
        private IEnumerator CheckAvailablePipeline()
        {
            // Fullbody avatars are available on the Pro plan. Need to verify it.
            SetControlsInteractable(false);
            var pipelineAvailabilityRequest = avatarProvider.IsPipelineSupportedAsync(selectedPipelineType);
            yield return Await(pipelineAvailabilityRequest);
            if (pipelineAvailabilityRequest.IsError)
                yield break;

            if (pipelineAvailabilityRequest.Result == true)
            {
                yield return UpdateAvatarParameters();
                progressText.text = string.Empty;
                SetControlsInteractable(true);
            }
            else
            {
                string errorMsg = "You can't generate fullbody avatars.\nThis option is available on the PRO plan.";
                progressText.text = errorMsg;
                progressText.color = Color.red;
                Debug.LogError(errorMsg);
            }
        }

        private IEnumerator ShowAvatarOnScene(string haircutName, string outfitName, FullbodyAvatarComputationParameters computationParameters)
        {
            avatarHolder = GameObject.Find(AVATAR_OBJECT_HOLDER);

            GameObject avatarObject = new GameObject(AVATAR_OBJECT_NAME);
            avatarObject.SetActive(false);


            if (SceneManager.GetActiveScene().name == "Avatar Viewer")
            {
                avatarObject.transform.parent = avatarHolder.transform;
            }
            else
            {
                //GameObject[] SupernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");
                //avatarObject.transform.parent = SupernovaPlayers[SupernovaPlayers.Length-1].transform;
                avatarObject.transform.parent = GameManager.Instance.avatars[avatarCode].gameObject.transform;
                GameManager.Instance.avatars[avatarCode].gameObject.GetComponent<DynamicAvatarFinder>().DynamicAvatar = avatarObject;

                //avatarObject.transform.parent = GameManager.Instance.lastCreatedDynamicAvatar.transform;
            }

            avatarObject.transform.localPosition = Vector3.zero;

            if (computationParameters.template == ExportTemplate.HEAD)
                avatarObject.transform.position = new Vector3(0, -0.2f, 1f);

            avatarLoader = new FullbodyAvatarLoader(fullbodyAvatarProvider);
            avatarLoader.AvatarGameObject = avatarObject;
            yield return avatarLoader.LoadAvatarAsync(currentAvatarCode);

            if (!string.IsNullOrEmpty(haircutName))
            {
                var showHaircutRequest = avatarLoader.ShowHaircutAsync(haircutName);
                yield return Await(showHaircutRequest);
            }

            if (!string.IsNullOrEmpty(outfitName))
            {
                var showOutfitRequest = avatarLoader.ShowOutfitAsync(outfitName);
                yield return Await(showOutfitRequest);
            }

            // avatarObject.AddComponent<MoveByMouse>();
            avatarObject.SetActive(true);
        }

        private void ConfigureHaircutsControls(string currentHaircutName)
        {
            haircuts = avatarLoader.GetHaircuts();
            haircuts.Sort();
            if (haircuts.Count > 0)
            {
                haircutsControlsParent.SetActive(true);

                haircuts.Insert(0, baldHaircutName);
                haircutsSelectingView.InitItems(haircuts);

                currentHaircutIdx = haircuts.IndexOf(currentHaircutName);
                if (currentHaircutIdx < 0)
                    currentHaircutIdx = 0;
                haircutNameText.text = haircuts[currentHaircutIdx];
            }
            else
                haircutsControlsParent.SetActive(false);
        }

        private void ConfigureOutfitsControls(string currentOutfitName)
        {
            outfits = avatarLoader.GetOutfits();
            outfits.Sort();
            if (outfits.Count > 0)
            {
                outfitsControlsParent.SetActive(true);

                outfits.Insert(0, emptyOutfitName);
                outfitsSelectingView.InitItems(outfits);

                currentOutfitIdx = outfits.IndexOf(currentOutfitName);
                if (currentOutfitIdx < 0)
                    currentOutfitIdx = 0;
                outfitNameText.text = outfits[currentOutfitIdx];
            }
            else
                outfitsControlsParent.SetActive(false);
        }

        private void ConfigureFacialAnimationsControls(List<string> blendshapesSets)
        {
            bool isMobileBlendshapesSetExist = blendshapesSets.Exists(s => s.Contains("mobile_51"));
            facialAnimationsControlsParent.SetActive(isMobileBlendshapesSetExist);
            if (isMobileBlendshapesSetExist)
            {
                string[] facialAnimationsNames = new string[]
                {
                    "smile", "blink", "kiss", "puff", "yawning", "fear", "distrust", "chewing", "mouth_left_right"
                };
                facialAnimationManager = avatarLoader.GetBodyMeshObject().AddComponent<AnimationManager>();
                facialAnimationManager.AssignAnimatorController(facialAnimatorController, facialAnimationsNames);
                facialAnimationManager.onAnimationChanged += name => facialAnimationNameText.text = name;
            }
        }

        private void ConfigureBodyAnimationsControls(bool bodyAnimationsAllowed, PipelineType pipelineType)
        {
            if (bodyAnimationsControlsParent.activeInHierarchy)
            {
                bodyAnimationsControlsParent.SetActive(bodyAnimationsAllowed);
            }

            if (bodyAnimationsAllowed)
            {
                string[] bodyAnimationsNames = new string[] { "A-Pose", "Waving", "Jumping", "Rumba Dancing", "Spin", "Arguing", "Dancing", "Walking" };
                bodyAnimationManager = avatarLoader.AvatarGameObject.AddComponent<FullbodyAnimationManager>();
                bodyAnimationManager.AssignAnimatorController(fullbodyAnimatorsHolder.GetAnimatorController(pipelineType), bodyAnimationsNames);
                bodyAnimationManager.onAnimationChanged += name => bodyAnimationNameText.text = name;

                List<string> outfitsWithHeels = new List<string>() { "outfit_0", "outfit_2", "outfit_0_lowpoly", "outfit_2_lowpoly", "outfit_meta_0", "outfit_meta_2" };
                if (avatarLoader.bodyAppearanceController != null)
                {
                    bodyAnimationManager.standOnHeels = outfitsWithHeels.Contains(avatarLoader.bodyAppearanceController.ActiveOutfitName);
                    avatarLoader.bodyAppearanceController.activeOutfitChanged += outfitName =>
                    {
                        bodyAnimationManager.standOnHeels = outfitsWithHeels.Contains(outfitName);
                    };
                }



            }
        }

        private IEnumerator UpdateAvatarParameters()
        {
            bool hideParametersPanel = isParametersPanelActive;
            if (hideParametersPanel)
                SwitchParametersPanelState();

            SetControlsInteractable(false);

            var parametersRequest = fullbodyAvatarProvider.GetAvailableComputationParametersAsync(selectedPipelineType);
            yield return Await(parametersRequest);
            if (parametersRequest.IsError)
            {
                Debug.LogError("Unable to get available computation parameters");
            }
            else
            {
                FullbodyAvatarComputationParameters availableParameters = parametersRequest.Result;
                // parametersPanel.UpdateParameters(availableParameters);
                SetControlsInteractable(true);
                if (hideParametersPanel)
                    SwitchParametersPanelState();
            }
        }

        private string GetInitialHaircutName(string avatarCode)
        {
            List<string> haircutsList = fullbodyAvatarProvider.GetHaircutsInDiscreteFiles(avatarCode);

            if (haircutsList == null || haircutsList.Count == 0)
                return string.Empty;

            if (haircutsList.Contains(generatedHaircutName))
                return generatedHaircutName;

            System.Random random = new System.Random(DateTime.Now.Millisecond);
            return haircutsList[random.Next(haircutsList.Count - 1)];
        }

        private string GetInitialOutfitName(string avatarCode)
        {
            List<string> outfitsList = fullbodyAvatarProvider.GetOutfitsInDiscreteFiles(avatarCode);

            if (outfitsList == null || outfitsList.Count == 0)
                return string.Empty;

            System.Random random = new System.Random(DateTime.Now.Millisecond);
            return outfitsList[random.Next(outfitsList.Count - 1)];
        }

        private IEnumerator ChangeHaircut(string haircutName)
        {
            SetControlsInteractable(false);

            if (haircutName == baldHaircutName)
                avatarLoader.HideAllHaircuts();
            else
            {
                var showHaircutRequest = avatarLoader.ShowHaircutAsync(haircutName);
                yield return Await(showHaircutRequest);
            }

            progressText.text = string.Empty;
            haircutNameText.text = haircutName;
            currentHaircutIdx = haircuts.IndexOf(haircutName);

            SetControlsInteractable(true);
        }

        private IEnumerator ChangeOutfit(string outfitName)
        {
            SetControlsInteractable(false);

            if (outfitName == emptyOutfitName)
                avatarLoader.HideAllOutfits();
            else
            {
                var showOutfitRequest = avatarLoader.ShowOutfitAsync(outfitName);
                yield return Await(showOutfitRequest);
            }

            progressText.text = string.Empty;
            outfitNameText.text = outfitName;
            currentOutfitIdx = outfits.IndexOf(outfitName);

            SetControlsInteractable(true);
        }

        private void SwitchParametersPanelState()
        {
            isParametersPanelActive = !isParametersPanelActive;
            parametersPanel.SwitchActiveState(isParametersPanelActive);

            progressText.gameObject.SetActive(!isParametersPanelActive);
            generatedAvatarControlsParent.SetActive(!isParametersPanelActive && isAvatarDisplayed);
        }
        #endregion private methods
    }
}
