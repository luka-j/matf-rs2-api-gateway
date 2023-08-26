import { z } from "zod";

export type FrontendBackendDetailArgs = {
  configType: FrontendBackend;
  apiName: string;
  apiVersion: string;
};

export type FrontendBackend = "frontend" | "backend";

export const frontendsBackendsConfigSchema = z.array(
  z.object({
    apiName: z.string(),
    apiVersion: z.string(),
    basePath: z.string(),
  }),
);

export const frontendsBackendsDetailSchema = z.object({
  data: z.string(),
  validFrom: z.string(),
});

export type FrontendsBackendsConfig = z.infer<typeof frontendsBackendsConfigSchema>;

export type FrontendsBackendsDetail = z.infer<typeof frontendsBackendsDetailSchema>;
