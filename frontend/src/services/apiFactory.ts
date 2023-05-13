/* eslint-disable @typescript-eslint/no-non-null-assertion */
import axios, { AxiosInstance, AxiosRequestConfig } from "axios";

import { useUserStore } from "@/stores/userStore";

const handleInterceptors = (apiInstance: AxiosInstance) => {
  apiInstance.defaults.headers.common["Content-Type"] = "application/json";

  apiInstance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.data) {
        if (error.response.status === 401 || error.response.status === 403) {
          useUserStore.getState().logoutUser();
        }
      }
      return Promise.reject(error);
    },
  );

  apiInstance.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("token");

      if (token) config.headers!["Authorization"] = `Bearer ${token}`;

      return config;
    },
    (error) => Promise.reject(error),
  );
};

interface IApiOptions extends AxiosRequestConfig {
  commonPrefix: string;
  port: number;
  prodURL: string;
}

const createApi = ({ port, commonPrefix, prodURL, ...rest }: IApiOptions) => {
  const api = axios.create({
    baseURL: import.meta.env.DEV
      ? `http://localhost:${port}/api/v1/${commonPrefix}/`
      : `${prodURL}/api/v1/${commonPrefix}/`,
    ...rest,
  });

  handleInterceptors(api);

  return api;
};

export default createApi;
