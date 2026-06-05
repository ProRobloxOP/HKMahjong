        static bool CanShowWizard()
        {
            // If the user has more than one SRP installed, only show the Wizard if the pipeline is HDRP
            return HDRenderPipeline.isReady;
            return HDRenderPipeline.isReady || RenderPipelineManager.currentPipeline == null;
        }

        void OnGUI();