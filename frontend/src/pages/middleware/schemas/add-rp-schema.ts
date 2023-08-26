import { z } from "zod";

export const addRpSchema = z.object({
  apiName: z.string().min(1, "API Name is required"),
  apiVersion: z.string().min(1, "API Version is required"),
  data: z.string(),
});

export type IAddRpSchema = z.infer<typeof addRpSchema>;
