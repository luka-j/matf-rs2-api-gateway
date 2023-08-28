import { useLocation } from "react-router-dom";
import SwaggerUI from "swagger-ui-react";

import "swagger-ui-react/swagger-ui.css";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { SubmitHandler, useForm } from "react-hook-form";

import { FrontendBackend } from "@/types/api-configs";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import CustomCodeEditor from "@/components/custom-code-editor";
import PageLoader from "@/components/page-loader";

import useCreateFrontendBackend from "./hooks/use-create-frontend-backend";
import useGetDetailFrontendBackend from "./hooks/use-get-detail-frontend-backend";
import {
  addFrontendBackendSchema,
  IAddFrontendBackendSchema,
} from "./schemas/add-frontend-backend-schema";

const Spec = () => {
  const location = useLocation();
  const { configType, apiName, apiVersion } = location.state as {
    configType: FrontendBackend;
    apiName: string;
    apiVersion: string;
    basePath: string;
  };

  const isEditing = !!apiName && !!apiVersion;

  const { data: detailFrontendBackend, isInitialLoading } = useGetDetailFrontendBackend({
    configType,
    apiName,
    apiVersion,
  });

  const { mutate: createFrontend, isLoading: isCreating } = useCreateFrontendBackend({
    isEditing,
    configType,
  });

  const form = useForm<IAddFrontendBackendSchema>({
    resolver: zodResolver(addFrontendBackendSchema),
    values:
      detailFrontendBackend && apiName && apiVersion
        ? {
            data: detailFrontendBackend.data,
            apiName: apiName,
            apiVersion: apiVersion,
          }
        : {
            apiName: "",
            apiVersion: "",
            data: "",
          },
  });
  const data = form.watch("data");

  if (isInitialLoading) return <PageLoader />;

  const onSubmit: SubmitHandler<IAddFrontendBackendSchema> = (data) => {
    createFrontend({ configType, ...data });
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="flex w-full flex-col">
        <div className="flex h-[80vh] w-full flex-col md:flex-row">
          <ScrollArea className="w-full rounded-md border md:w-1/2">
            <CustomCodeEditor data={data} setData={form.setValue} />
          </ScrollArea>

          <ScrollArea className="w-full rounded-md border md:w-1/2">
            <SwaggerUI spec={data} />
          </ScrollArea>
        </div>
        <div className="mt-4 flex justify-center gap-4">
          <FormField
            control={form.control}
            name="apiName"
            render={({ field }) => (
              <FormItem>
                <FormControl>
                  <Input placeholder={`${configType} name...`} {...field} disabled={isEditing} />
                </FormControl>
                <FormDescription>This is the name of your {configType}.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="apiVersion"
            render={({ field }) => (
              <FormItem>
                <FormControl>
                  <Input placeholder={`${configType} version...`} {...field} disabled={isEditing} />
                </FormControl>
                <FormDescription>This is the version of your {configType}.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button className="px-8" type="submit" disabled={isCreating}>
            {isCreating ? <Loader2 className="animate-spin" /> : "Save & Publish"}
          </Button>
        </div>
      </form>
    </Form>
  );
};

export default Spec;
