import { z } from "zod";

export type DatasourceType = "databases" | "caches" | "queues";

const baseDatasourceObjectSchema = z.object({
  type: z.string(),
  url: z.string(),
  password: z.string(),
});

const baseDatasourceSchema = z.object({
  title: z.string(),
  datasource: baseDatasourceObjectSchema,
});

export const databaseSchema = baseDatasourceSchema.extend({
  datasource: baseDatasourceObjectSchema.extend({
    port: z.number(),
    databaseName: z.string(),
    username: z.string(),
  }),
});

export const cacheSchema = baseDatasourceSchema.extend({
  datasource: baseDatasourceObjectSchema,
});

export const queueSchema = baseDatasourceSchema.extend({
  datasource: baseDatasourceObjectSchema.extend({
    username: z.string(),
  }),
});

export const ccoConfigsSchema = z.object({
  databases: z.array(databaseSchema),
  caches: z.array(cacheSchema),
  queues: z.array(queueSchema),
});

export type Database = z.infer<typeof databaseSchema>;

export type Cache = z.infer<typeof cacheSchema>;

export type Queue = z.infer<typeof queueSchema>;

export type Datasource = Database | Cache | Queue;

export type CCOConfigs = z.infer<typeof ccoConfigsSchema>;
