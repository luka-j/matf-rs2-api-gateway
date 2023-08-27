import { ccoConfigsSchema, Datasource, DatasourceType } from "@/types/cco-config";

import createApi from "./api-factory";

const ccoConfigApi = createApi({ commonPrefix: "cco" });

const getDatasources = async () => {
  return await ccoConfigApi.get("").then((res) => ccoConfigsSchema.parse(res.data));
};

const createDatasource = async (data: { type: DatasourceType; data: Datasource }) => {
  return await ccoConfigApi.post(data.type, data.data);
};

const deleteDatasource = async (data: { type: DatasourceType; title: string }) => {
  return await ccoConfigApi.delete(`${data.type}/${data.title}`);
};

const ccoConfigService = {
  getDatasources,
  createDatasource,
  deleteDatasource,
};

export default ccoConfigService;

export const ccoConfigKey = "ccoConfig";
