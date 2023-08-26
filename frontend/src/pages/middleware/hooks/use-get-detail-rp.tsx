import { useQuery } from "@tanstack/react-query";

import { RpConfig } from "@/types/rp-configs";
import rpConfigService, { rpConfigKey } from "@/services/rp-config.service";

const useGetDetailRp = ({ apiName, apiVersion }: RpConfig) => {
  return useQuery({
    queryKey: [rpConfigKey, apiName, apiVersion],
    queryFn: () => rpConfigService.getDetailRp({ apiName, apiVersion }),
  });
};

export default useGetDetailRp;
