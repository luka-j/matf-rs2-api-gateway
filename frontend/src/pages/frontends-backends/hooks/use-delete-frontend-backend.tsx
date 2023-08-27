import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import { FrontendBackend } from "@/types/api-configs";
import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

const useDeleteFrontendBackend = (frontOrBack: FrontendBackend) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: apiConfigService.deleteFrontendBackend,
    onSuccess: () => {
      queryClient.invalidateQueries([apiConfigKey, frontOrBack]);
      toast.success(
        `${frontOrBack.slice(0, 1).toUpperCase() + frontOrBack.slice(1)} will be deleted soon.`,
      );
    },
  });
};

export default useDeleteFrontendBackend;
