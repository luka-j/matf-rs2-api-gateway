import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FaTrash } from "react-icons/fa";

import { Input } from "@/components/Input";

import { editDatasourceSchema, IEditDatasourceSchema } from "../schemas/EditDatasourceSchema";

interface IDatasourceCardProps {
  name: string;
  type: string;
  url: string;
  username: string;
}

const DatasourceCard = (props: IDatasourceCardProps) => {
  const { name, type, url, username } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IEditDatasourceSchema>({
    resolver: zodResolver(editDatasourceSchema),
    defaultValues: props,
  });

  const onSubmit: SubmitHandler<IEditDatasourceSchema> = (data) => {
    console.log(data);
  };

  const [editState, setEditState] = useState(false);

  return (
    <div className="mt-12 w-full max-w-sm p-4 border rounded-lg shadow sm:p-8 bg-gray-800 border-gray-700">
      {editState ? (
        <form
          onSubmit={handleSubmit(onSubmit)}
          className="text-sm grid grid-cols-1 md:grid-cols-2 gap-4"
        >
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
          <Input
            placeholder="Password"
            {...register("password")}
            state={errors?.password ? "error" : undefined}
            errorMessage={errors?.password?.message}
          />
          <div className="flex w-full gap-2 items-center">
            <button className="hover:bg-white/75 rounded-full w-fit h-fit p-1 transition-colors duration-300">
              <FaTrash className="text-red-600 hover:text-red-700" />
            </button>
            <button
              className="text-sm w-full text-center text-blue-600 hover:text-blue-700"
              onClick={() => setEditState(!editState)}
              type="reset"
            >
              Cancel
            </button>
            <button
              className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm w-full text-center mr-3 md:mr-0 py-1.5 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800 text-black"
              type="submit"
            >
              Save
            </button>
          </div>
        </form>
      ) : (
        <>
          <div className="flex flex-col gap-4 items-center justify-center mb-4">
            <h5 className="text-xl font-bold leading-none ">{name}</h5>
            <h6 className="text-lg font-semibold leading-none ">{type}</h6>
            <p className="leading-none">{url}</p>
            <p className="leading-none">{username}</p>
          </div>
          <div className="flex justify-end mt-8">
            <button
              className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm w-1/3 py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800 text-black"
              onClick={() => setEditState(!editState)}
            >
              Edit
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default DatasourceCard;
