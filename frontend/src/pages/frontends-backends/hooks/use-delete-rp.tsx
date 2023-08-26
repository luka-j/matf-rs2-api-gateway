import { useMutation, useQueryClient } from "@tanstack/react-query";

import rpConfigService, { rpConfigKey } from "@/services/rp-config.service";

const useDeleteRp = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: rpConfigService.deleteRp,
    onSuccess: () => {
      queryClient.invalidateQueries([rpConfigKey]);
    },
  });
};

export default useDeleteRp;
