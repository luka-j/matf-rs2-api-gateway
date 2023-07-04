import { zodResolver } from "@hookform/resolvers/zod";
import { SubmitHandler, useForm } from "react-hook-form";

import { Input } from "@/components/Input";

import { addDatasourceSchema, IAddDatasourceSchema } from "../schemas/AddDatasourceSchema";

interface IAddModalContentProps {
  type: string;
  handleClose: () => void;
}

const AddModalContent = ({ type, handleClose }: IAddModalContentProps) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IAddDatasourceSchema>({
    resolver: zodResolver(addDatasourceSchema),
  });

  const onSubmit: SubmitHandler<IAddDatasourceSchema> = (data) => {
    console.log(data);
    handleClose();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-2">
      <p className="text-2xl font-bold mb-4 text-center capitalize">Add {type}</p>
      <Input
        placeholder="Name"
        {...register("name")}
        state={errors?.name ? "error" : undefined}
        errorMessage={errors?.name?.message}
      />
      <Input
        placeholder="Type"
        {...register("type")}
        state={errors?.type ? "error" : undefined}
        errorMessage={errors?.type?.message}
      />
      <Input
        placeholder="URL"
        {...register("url")}
        state={errors?.url ? "error" : undefined}
        errorMessage={errors?.url?.message}
      />
      <Input
        placeholder="Username"
        {...register("username")}
        state={errors?.username ? "error" : undefined}
        errorMessage={errors?.username?.message}
      />
      <button className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm w-full py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800 text-black">
        Add Datasource
      </button>
    </form>
  );
};

export default AddModalContent;
