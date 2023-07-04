import { cva, VariantProps } from "class-variance-authority";
import React from "react";
import { FcCheckmark } from "react-icons/fc";
import { VscError } from "react-icons/vsc";

import { cn } from "@/utils/styleUtils";

export const inputVariants = cva(
  "bg-transparent outline-none placeholder:font-normal w-full flex disabled:font-light disabled:placeholder:font-light mr-1 overflow-auto",
);

export const inputContainerVariants = cva(
  [
    "relative mb-2 flex items-center rounded py-2 pl-4 text-base border border-opacity-50",
    "border-blue-700",
    "focus-within:border-blue-700 hover:focus-within:border-opacity-100",
  ],
  {
    variants: {
      state: {
        error:
          "border-error border border-opacity-100 hover:border-error hover:border-opacity-100 focus-within:border-error focus-within:border-opacity-100",
        success:
          "border-success border border-opacity-100 hover:border-success hover:border-opacity-100 focus-within:border-success focus-within:border-opacity-100",
      },
    },
  },
);

export interface InputProps
  extends React.InputHTMLAttributes<HTMLInputElement>,
    VariantProps<typeof inputVariants>,
    VariantProps<typeof inputContainerVariants> {
  StartIcon?: (iconProps: React.SVGProps<SVGSVGElement>) => JSX.Element;
  startIconProps?: React.SVGProps<SVGSVGElement>;
  EndIcon?: (iconProps: React.SVGProps<SVGSVGElement>) => JSX.Element;
  endIconProps?: React.SVGProps<SVGSVGElement>;
  successMessage?: string;
  errorMessage?: string;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  (
    {
      state,
      StartIcon,
      startIconProps,
      EndIcon,
      endIconProps,
      successMessage,
      errorMessage,
      disabled,
      className,
      ...props
    },
    ref,
  ) => {
    return (
      <div className="flex flex-col">
        <div
          className={cn(
            inputContainerVariants({ state, className }),
            disabled && "pointer-events-none bg-opacity-20 text-opacity-20",
          )}
        >
          {StartIcon && (
            <div className="mr-2 flex justify-center">
              <StartIcon {...startIconProps} />
            </div>
          )}
          <input className={cn(inputVariants())} ref={ref} {...props} />
          {EndIcon && (
            <div className="mr-4 flex justify-center">
              <EndIcon {...endIconProps} />
            </div>
          )}
        </div>
        {successMessage && (
          <p className="inline-flex flex-row items-center justify-start gap-2 text-success">
            <FcCheckmark /> {successMessage}
          </p>
        )}
        {errorMessage && (
          <p className="inline-flex flex-row items-center justify-start gap-2 text-error">
            <VscError /> {errorMessage}
          </p>
        )}
      </div>
    );
  },
);

Input.displayName = "Input";
