import { useQuery } from "@tanstack/react-query";

import rpConfigService, { rpConfigKey } from "@/services/rp-config.service";

const useGetRps = () => {
  return useQuery({
    queryKey: [rpConfigKey],
    queryFn: () => rpConfigService.getRps(),
  });
};

export default useGetRps;
