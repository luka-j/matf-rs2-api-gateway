import { z } from "zod";

export type DatasourceType = "databases" | "caches" | "queues";

export const datasourceSchema = z.object({
  title: z.string(),
  datasource: z.object({
    type: z.string(),
    url: z.string(),
    username: z.string(),
    password: z.string(),
    connectionString: z.string(),
  }),
});

export const ccoConfigsSchema = z.object({
  databases: z.array(datasourceSchema),
  caches: z.array(datasourceSchema),
  queues: z.array(datasourceSchema),
});

export type Datasource = z.infer<typeof datasourceSchema>;

export type CCOConfigs = z.infer<typeof ccoConfigsSchema>;
