import { IAddRpSchema } from "@/pages/middleware/schemas/add-rp-schema";

import { RpConfig, rpDetailSchema, rpsConfigSchema } from "@/types/rp-configs";

import createApi from "./api-factory";

const rpConfigApi = createApi({ commonPrefix: "rp" });

const getRps = async () => {
  return await rpConfigApi.get("").then((res) => rpsConfigSchema.parse(res.data));
};

const getDetailRp = async ({ apiName, apiVersion }: RpConfig) => {
  return await rpConfigApi
    .get(`${apiName}/${apiVersion}`)
    .then((res) => rpDetailSchema.parse(res.data));
};

const deleteRp = async ({ apiName, apiVersion }: RpConfig) => {
  return await rpConfigApi.delete(`${apiName}/${apiVersion}`);
};

const createRp = async (data: IAddRpSchema) => {
  return await rpConfigApi.post("", data);
};

const rpConfigService = {
  getRps,
  getDetailRp,
  deleteRp,
  createRp,
};

export default rpConfigService;

export const rpConfigKey = "rpConfig";
