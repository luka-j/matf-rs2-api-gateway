import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import rpConfigService, { rpConfigKey } from "@/services/rp-config.service";

const useDeleteRp = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: rpConfigService.deleteRp,
    onSuccess: () => {
      queryClient.invalidateQueries([rpConfigKey]);
      toast.success("Rp will be deleted soon.");
    },
  });
};

export default useDeleteRp;
