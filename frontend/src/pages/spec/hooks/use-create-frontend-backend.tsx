import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import { FrontendBackend } from "@/types/api-configs";
import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

interface IUseCreateFrontendBackendProps {
  isEditing: boolean;
  configType: FrontendBackend;
}
const useCreateFrontendBackend = ({ isEditing, configType }: IUseCreateFrontendBackendProps) => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: apiConfigService.createFrontendBackend,
    onSuccess: () => {
      queryClient.invalidateQueries([apiConfigKey, configType]);
      toast.success(
        `${configType.slice(0, 1).toUpperCase() + configType.slice(1)} will be ${
          isEditing ? "edited" : "created"
        } soon.`,
      );
      navigate(`/dashboard/${configType}s`);
    },
  });
};

export default useCreateFrontendBackend;
