import { IAddFrontendBackendSchema } from "@/pages/spec/schemas/add-frontend-backend-schema";

import {
  FrontendBackend,
  FrontendBackendDetailArgs,
  frontendsBackendsConfigSchema,
  frontendsBackendsDetailSchema,
} from "@/types/api-configs";

import createApi from "./api-factory";

const apiConfigApi = createApi({ commonPrefix: "api" });

const getFrontendsBackends = async (configType: FrontendBackend) => {
  return await apiConfigApi
    .get(configType)
    .then((res) => frontendsBackendsConfigSchema.parse(res.data));
};

const getDetailFrontendBackend = async ({
  configType,
  apiName,
  apiVersion,
}: FrontendBackendDetailArgs) => {
  return await apiConfigApi
    .get(`${configType}/${apiName}/${apiVersion}`)
    .then((res) => frontendsBackendsDetailSchema.parse(res.data));
};

const deleteFrontendBackend = async ({
  configType,
  apiName,
  apiVersion,
}: FrontendBackendDetailArgs) => {
  return await apiConfigApi.delete(`${configType}/${apiName}/${apiVersion}`);
};

const createFrontendBackend = async (
  data: IAddFrontendBackendSchema & { configType: FrontendBackend },
) => {
  return await apiConfigApi.post(data.configType, data);
};

const apiConfigService = {
  getFrontendsBackends,
  getDetailFrontendBackend,
  deleteFrontendBackend,
  createFrontendBackend,
};

export default apiConfigService;

export const apiConfigKey = "apiConfig";
