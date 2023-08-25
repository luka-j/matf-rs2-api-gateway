import { z } from "zod";

export const addFrontendBackendSchema = z.object({
  apiName: z.string().min(1, "API Name is required"),
  apiVersion: z.string().min(1, "API Version is required"),
  data: z.string(),
});

export type IAddFrontendBackendSchema = z.infer<typeof addFrontendBackendSchema>;
