import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import rpConfigService, { rpConfigKey } from "@/services/rp-config.service";

const useCreateRp = () => {
  const navigate = useNavigate();

  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: rpConfigService.createRp,
    onSuccess: () => {
      queryClient.invalidateQueries([rpConfigKey]);
      toast.success(`Successfully edited RP!`);
      navigate("/dashboard/frontends");
    },
  });
};

export default useCreateRp;
