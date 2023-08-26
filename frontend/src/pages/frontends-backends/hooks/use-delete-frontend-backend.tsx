import { useMutation, useQueryClient } from "@tanstack/react-query";

import { FrontendBackend } from "@/types/api-configs";
import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

const useDeleteFrontendBackend = (frontOrBack: FrontendBackend) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: apiConfigService.deleteFrontendBackend,
    onSuccess: () => {
      queryClient.invalidateQueries([apiConfigKey, frontOrBack]);
    },
  });
};

export default useDeleteFrontendBackend;
