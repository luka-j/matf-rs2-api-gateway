import { zodResolver } from "@hookform/resolvers/zod";
import { SubmitHandler, useForm } from "react-hook-form";
import { useLocation } from "react-router-dom";

import PageLoader from "@/components/page-loader";

import useCreateRp from "./hooks/use-create-rp";
import useGetDetailRp from "./hooks/use-get-detail-rp";
import { addRpSchema, IAddRpSchema } from "./schemas/add-rp-schema";

import "swagger-ui-react/swagger-ui.css";

import { Loader2 } from "lucide-react";

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

const Middlewares = () => {
  const location = useLocation();
  const { apiName, apiVersion } = location.state as {
    apiName: string;
    apiVersion: string;
  };

  const { data: detailRp, isLoading } = useGetDetailRp({
    apiName,
    apiVersion,
  });

  const { mutate: createRp, isLoading: isCreating } = useCreateRp();

  const form = useForm<IAddRpSchema>({
    resolver: zodResolver(addRpSchema),
    values: {
      data: detailRp?.data ?? "",
      apiName: apiName,
      apiVersion: apiVersion,
    },
  });
  const data = form.watch("data");

  if (isLoading) return <PageLoader />;

  const onSubmit: SubmitHandler<IAddRpSchema> = (data) => {
    createRp(data);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="flex w-full flex-col">
        <div className="mt-4 flex h-[80vh] w-full justify-center">
          <ScrollArea className="w-1/2 rounded-md border">
            <CustomCodeEditor data={data} setData={form.setValue} />
          </ScrollArea>
        </div>
        <div className="mt-4 flex justify-center gap-4">
          <FormField
            control={form.control}
            name="apiName"
            render={({ field }) => (
              <FormItem>
                <FormControl>
                  <Input placeholder={"Middleware name..."} {...field} disabled />
                </FormControl>
                <FormDescription>This is the name of your middleware.</FormDescription>
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
                  <Input placeholder={"Middleware version..."} {...field} disabled />
                </FormControl>
                <FormDescription>This is the version of your middleware.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button className="px-8" type="submit" disabled={isCreating}>
            {isCreating ? <Loader2 className="animate-spin" /> : "Edit middleware"}
          </Button>
        </div>
      </form>
    </Form>
  );
};

export default Middlewares;
