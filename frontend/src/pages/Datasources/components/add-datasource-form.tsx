import { zodResolver } from "@hookform/resolvers/zod";
import { SubmitHandler, useForm } from "react-hook-form";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import { addDatasourceSchema, IAddDatasourceSchema } from "../schemas/add-datasource-schema";

interface IAddDatasourceFormProps {
  type: string;
}

const AddDatasourceForm = ({ type }: IAddDatasourceFormProps) => {
  const form = useForm<IAddDatasourceSchema>({
    resolver: zodResolver(addDatasourceSchema),
  });

  const onSubmit: SubmitHandler<IAddDatasourceSchema> = (data) => {
    console.log(data);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Name</FormLabel>
              <FormControl>
                <Input placeholder={`${type}1`} {...field} />
              </FormControl>
              <FormDescription>This is the name of your {type}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="type"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Type</FormLabel>
              <FormControl>
                <Input placeholder={`${type} type 1`} {...field} />
              </FormControl>
              <FormDescription>This is the type of your {type}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="url"
          render={({ field }) => (
            <FormItem>
              <FormLabel>URL</FormLabel>
              <FormControl>
                <Input placeholder={`${type}://localhost:10000`} {...field} />
              </FormControl>
              <FormDescription>This is the URL to your {type}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="username"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Username</FormLabel>
              <FormControl>
                <Input placeholder="tom12345@admin" {...field} />
              </FormControl>
              <FormDescription>This is the username you will use to connect.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" className="w-full">
          Add {type}
        </Button>
      </form>
    </Form>
  );
};

export default AddDatasourceForm;
